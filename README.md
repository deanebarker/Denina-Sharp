# Text Filtering Pipeline

(Note: be sure to read the "History and Context" section at the end for more information about why this project was created, and what it means for you.)

TFP is a C# pipeline processor for text, intended for editorial usage through configuration by simple text commands. A pipeline is a series of filters, processed in sequential order.  In most cases, the output from one filter is the input to the filter immediately following it (this is the "active text").

The filters are linear and sequential.  Text is passed "down the line," and is usually modified during each step, coming out the other end in a different form than when it started.

## The Basics

Pretend for a moment that we have our name in a text string ("Deane"). We want to add some other text to this, and then format that for HTML.

To do this, we can create these commands:

    Text.Prepend "My name is: "
    Text.Append "."
    Html.Wrap p

What this says, in order, is:

1. Put the text "My name is: " before the input (what we pass into the pipeline -- our name)
2. Put a period after the result of #1
3. Wrap the result of #2 in P tag
4. Return the result of #3 (this step is implied -- the pipeline always returns the result of the last operation)

Notice that each step expands or builds on the result of the previous step. The input that goes into #1 gets passed down the pipe and altered, step after step.

If we pass "Deane" to the pipeline, it comes out:

    <p>My name is Deane.</p>

If we pass "Annie" to the same pipeline, we'd get...

    <p>My name is Annie.</p>

Once the pipeline is configured, it stands ready for us to "throw" what ever we like down it.  We could pass the entire text of "War and Peace" down this same pipeline and it would come out.

    <p>My name is Well, Prince, so Genoa and Lucca are now ...</p>

This is the traditional usage of a pipeline -- we have some text "in hand," and want to process it.

However, it isn't always necessary for us to start with some actual text. Perhaps we don't have text, but we have the _means to get some text_, like the path to a file or a URL.

Using this, some pipeline commands can obtain text in-process.  For instance, if we wanted to format and output the contents of a file on the file system, we could do something like this:

    File.Read my-file.txt
    Text.Replace foo bar
    Text.Format "The contents of the file are: {0}."
    Html.Wrap p

That would read in the contents of "my-file.txt," replace the string "foo" with "bar," drop the result into the middle of a sentence, and again wrap it in a P tag.  In this case we don't pass anything into the pipeline -- it obtains text to work with in the first step.  (If we did pass something in, it would simply get discarded and replace in that first step.)

Filters are grouped into categories which do different things.  For example, the "HTTP" category can make web requests, and the "HTML" category can manipulate HTML documents.  Combine them, and you can do things like this:

    Http.Get gadgetopia.com
    Html.Extract //title
    Text.Format "The title of this web page is {0}."

Http.Get makes a -- wait for it -- GET request over HTTP to the URL specified in the first argument and returns the HTML. Html.Extract uses an external library to reach into the HTML and grab a value.  Text.Format, as we saw before, wraps this value within other text.

(See "Variables" below for a more extensive and practical example of working over HTTP.)

There are two programming "levels" to this library.

* There's the C# level, which instantiates the pipeline, passes data to it, and does something with the result. This tends to be fairly static -- it will be implemented once, in a way to make it available for editors (those using a CMS, for example -- this was the original intent; see "History and Context" at the end of this document).
* Then there's the filter configuration level, which involves setting up the filters and telling them what to do.  This level requires (1) knowing the format for calling filters and passing arguments; and (2) knowing what filters are available, what information they need, and what results they will return.

The first level is intended for C# developers.  The second level is intended for non-developers -- primarily content editors that need to obtain and modify text-based content for publication, without the assistance of a developer.

## The <span>C#</span>

(Note: The words "command" and "filter" get used interchangeably in this document. Technically, a "command" is an object that invokes and configures a "filter," which is a method. In practice, I'll go back and forth between the terms indiscriminately. Sorry.)

Here's the C# to instantiate the pipeline and add a command, the long way.

    var pipeline = new TextFilterPipeline();
    pipeline.AddCommand(
       new TextFilterCommand()
       {
         CommandName = "Text.Prepend",
         CommandArgs = new Dictionary<object,string>() { { 1,"FOO" } }
       }
      );
    var result = pipeline.Execute("BAR");
    // Result contains "FOOBAR"

Clearly, this is way too verbose.  So commands can be added by simple text strings.  The strings are tokenized on whitespace. The first token is the command name, the subsequent tokens are arguments. (Any arguments which contain whitespace need to be in quotes.)

    var pipeline = new TextFilterPipeline();
    pipeline.AddCommand("Text.Prepend FOO");  //Note: this can also be passed into the constructor
    var result = pipeline.Execute("BAR");

The result will be "FOOBAR".

We can shorten it even more by passed commands into the constructor:

    var pipeline = new TextFilterPipeline("Text.Prepend FOO");

In fact, commands can be passed in _en masse_, separated by line breaks (note that command parsing is broken out to its own class, and could easily be re-implemented, if you wanted to do something different).  Each line is parsed as a separate command.

    var pipeline = new TextFilterPipeline(thousandsAndThousandsOfCommands);

The pipeline remains "loaded" with commands even after execution, so we could just as easily do this immediately after:

    pipeline.Execute("BAZ");

We'd get "FOOBAZ."  We could pass a thousand different strings to the pipeline, and they would all come out with "FOO" prepended to them.

"Prepend" is one example of several dozen pre-built filters. Some take arguments, some don't. It's up to the individual filter how many arguments it needs, what order it needs them in, and what it does with them during execution (much like function calls in any programming language).

The pipeline doesn't have to start with text, as some filters allow the pipeline to acquire text mid-stream. In these cases, the pipeline is invoked without arguments.

    pipeline.Execute();
    
How a filter "treats" the active text is up to the filter. _The active text becomes whatever the filter returns._  It can return some derivation of the input text (such as with "Text.Prepend," from above), or it can return something completely new without regard to the active text it took in. It can even use the active text to configure itself and then return something else. (Example: if invoked without arguments, Http.Get assumes the active text is the URL it should use. It retrieves the HTML at that URL, and returns the result.)

In our example above, after the first filter (Http.Get) executes, the active text is _all_ the HTML from the home page of Gadgetopia.  After the second filter (Html.Extract) executes, the active text is just the contents of the "title" tag.  The active text after the last filter executes is what is returned by the Execute method of the pipeline object (it's what comes out "the other end" of the pipe).

Filters are grouped into categories (think "namespaces").  Any command without a "dot" is assumed to map to "Core" category.

## Variables

By default, a filter changes the active text and passes it to the next filter. However, the result of a filter can be instead redirected into variable which is stored for later use.  This does _not_ change the active text -- it remains unchanged.

You can direct the result of an operation to a variable by using the "=>" operator and a variable name at the end of a statement.  Variable names start with a dollar sign ("$").

Here's an example of chaining filters and writing into and out of variables to obtain and format the temperature in Sioux Falls:

    Http.Get http://api.openweathermap.org/data/2.5/weather?q=Sioux+Falls&mode=xml&units=imperial
    Xml.Extract //city/@name => $city
    Xml.Extract //temperature/@value => $temp
    Text.Format "The temp in {city} is {temp}."
    Html.Wrap p weather-data

The first command gets an XML document. Since the second command sends the results to a variable named $city, the active text remains the original full XML document which is then still available to the third command.

(Note that in this case, that XML document is going to be fully parsed twice from the string source, which may or may not work for your situation, performance-wise. Remember that filters only pass simple text, not more complex objects.)

The result of this pipeline is:

    <p class="weather-data">The temp in Sioux Falls is 37.</p>

Variables are mutable -- writing to the same variable multiple times simply resets the value each time.

Attempting to retrieve a variable before it exists will result in an error.  Initialize variables to avoid this using InitVar:

    InitVar $myVar $myOtherVar

This sets both $myVar and $myOtherVar to empty strings.

To manually set a variable value, use SetVar.

    SetVar $name Deane

This sets the value of $name to "Deane".

## Writing Filters

Filters are pluggable. Simply write a method, like this:

      public static string Left(string input, TextFilterCommand command)
      {
        var length = int.Parse(command.CommandArgs[0]);
        return input.Substring(0, length);
      }

The method needs to take in two arguments:

1. **String:** this is the input; what is passed to the filter
2. **TextFilterCommand:** this is an object representation of the line of text used to call the filter. On it are properties to access the command name and the arguments.

The method does whatever it wants to the input string, and returns the result as another string.  The method doesn't need to worry about writing into or out of variables -- those actions are handled by the pipeline itself.

Then register the MethodInfo object with the pipeline, telling it the category and name of the filter.

	var method = typeOf(MyClass).GetMethod("Left");
    TextFilterPipeline.AddMethod(method, "Text", "Left");

After registering, your command is now available as:

    Text.Left 10

The category and name arguments are optional. If you include the TextFilter attribute on method declaration, you don't have to pass that in:

    [TextFilter("Left")]

Alternately, you can register an entire type full of methods:

    TextFilterPipeline.AddType(typeof(MyFilters), "Text")

That will search the type for all methods with a TextFilter attribute.  Like with methods, you can identify the category with an a TextFilters (note the plural) attribute on the class, and you don't have to pass the category name in:

    [TextFilters("Text")]

Then register like this:

    TextFilterPipeline.AddType(typeof(MyFilters)

Finally, you can even register entire assemblies at a time, if all your filters are in a separate DLL:

    var myAssembly = Assembly.LoadFile(@"C:\MyFilters.dll");
    TextFilterPipeline.AddAssembly(myAssembly);

All the types in that assembly marked with the "TextFilters" attribute will be searched for methods with the "TextFilter" attribute.  (In fact, when the TextFilterPipeline class first initializes, it loads its built-in filters by simply passing the currently executing assembly to AddAssembly.)

Note that the name of the underlying C# method is irrelevant.  The filter maps to the combination of the category name ("Text," in this case) and filter ("Left"), both supplied by the attributes. While it would make sense to call the method the same name as the filter, this isn't required.

In the example above case, we're trusting that this filter will be called with (1) at least one argument (any extra arguments are simply ignored), (2) that the argument will parse to an Int32, and (3) that the numeric value isn't longer than the active text.  Clearly, _you're gonna want to validate and error check this inside your filter before doing anything_.

And what happens if there's an error condition?  Do you return the string unchanged?  Do you throw an exception?  That's up to you, but there is no user interaction during pipeline execution, so error conditions are problematic.

You can map the same filter to multiple command names, then use that name inside the method to change execution.

    [TextFilters("Text")]
    public static class TextFilters
    {
      [TextFilter("Left")]
      [TextFilter("Right")]
      public static string Left(string input, TextFilterCommand command)
      {
        var length = int.Parse(command.CommandArgs[0]);

        if(command.CommandName == "Left")
        {
          return input.Substring(0, length);
        }
        else
        {
          return input.Substring(input.Length - length);
        }
      }
    }

This filter will map to both of these commands:

    Text.Left 10
    Text.Right 10

Note that this is true even though the method name ("Left") did not change.

If your category and command name are identical to another one, the last one in wins. This means you can "hide" previous filters by registering new ones that take their place.  New filters are loaded statically, so they're globally available to all executions of the pipeline.

## Contents

This repo contains three projects.

1. The source to create a DLL named "BlendInteractive.TextFilterPipeline.dll"
2. A test project with moderate unit test coverage (needs to be better)
3. A WinForms testing app which provides a GUI to run and test filters.

On build, the DLL, a supporting DLL (HtmlAgilityPack), and the WinForms EXE are copied into the "Binaries" folder. The WinForms tester should run directly from there.

## History and Context

This started out as a simple project to allow editors to include the contents of a text file within [EPiServer](http://episerver.com) content.

That CMS provides "blocks," which are reusable content elements.  I wrote a simple block into which a editor could specify the path to a file on the file system. The block would read the contents of the file and dump it into the page.  It was essentially a server-side file include for content editors.

Then I got to thinking (always dangerous) that some files might need to have newlines replaced with BR tags, and how would I specify that?  And what if the file wasn't local?  How would I specify a remote file?  And what if it was XML -- could I specify a transform?

And the idea of a text filter pipeline was born.  To support this, I needed to come up with language constructs, and that's when I started parsing commands. And then I found I could enable some really neat functionality by tweaking and tuning and making small changes.

And when the snowball finally came to a rest at the bottom of the hill, you had, well, this.

The constant challenge with this type of project is knowing when to stop. At what point are you simply inventing a new programming language?  When do you cross the line from simple and useful to pointless and redundant?  And when do you cross another line into something which is potentially dangerous in the hands of non-programmers?

Look back to the weather example from above -- that really has nothing to do with text filtering. The pipeline is executed without input, the XML is obtained in the first step, and content is extracted then formatted. In this case, we're not filtering at all. We're really edging into a simplistic procedural programming language. How far is too far?  At what point does [Alan Turing roll over in his grave](http://stackoverflow.com/questions/7284/what-is-turing-complete)?

I don't have an answer for that (hell, we may have crossed the line already).  I leave it to you to judge.

Implement with care. Happy Filtering.

Deane Barker, January 2015
