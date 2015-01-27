# Text Filtering Pipeline

TFP is a pipeline processor for text, intended for editorial usage through configuration by simple text commands. A pipeline is a series of filters, processed in sequential order.  In most cases, the output from one filter is the input to the filter immediately following it (this is the "active text").

(Note: be sure to read the "History" section at the end for more context about why this project was created.)

The filters are linear and sequential.  Text is passed "down the line," and is usually modified during each step, coming out the other end in a different form than when it started.

Say we have the string "BAR" and we'd like to prepend FOO to it.  We create a pipeline, and add a command to invoke the "Prepend" filter, passing it "FOO" as the first argument.

(The words "command" and "filter" get used interchangably in this document. Technically, a "command" is an object that invokes and configures a "filter," which is a method. In practice, I'll go back and forth between the terms indiscriminately. Sorry.)

Here's the C#:

    var pipeline = new TextFilterPipeline();
    pipeline.AddCommand(
       new TextFilterCommand()
       {
         CommandName = "Prepend",
         CommandArgs = new Dictionary<object,string>() { { 1,"FOO" } }
       }
      );
    var result = pipeline.Execute("BAR");
    // Result contains "FOOBAR"

Clearly, this is way too verbose.  So commands can be added by simple text strings.  The strings are tokenized on whitespace. The first token is the command name, the subsequent tokens are arguments. (Any arguments which contain whitespace need to be in quotes.)

    var pipeline = new TextFilterPipeline();
    pipeline.AddCommand("Prepend FOO")
    var result = pipeline.Execute("BAR");

The result will be "FOOBAR".

The pipeline remains "loaded" with commands, so we could just as easily do this immediately after:

    pipeline.Execute("BAZ");

We'd get "FOOBAZ."

"Prepend" is one example of several dozen pre-built commands. Some take arguments, some don't. It's up to the individual commands how many arguments they need, what order they need them in, and what they do with them (much like function calls in any programming language).

The pipeline doesn't technically have to start with text, as some commands can allow the pipeline to acquire text mid-stream.  For example, the command configuration to call the home page of Gadgetopia and extract the title looks like this:

    Http.Get gadgetopia.com
    Html.Extract //title

In this case, the pipeline is simply invoked without arguments.

    pipeline.Execute();

After the first command executes, the active text is _all_ the HTML from the home page of Gadgetopia.  After the second command executes, the active text is just the contents of the "title" tag.  The active text after the last command executes is what is returned by the pipeline (it's what comes out "the other end" of the pipe).

Commands are grouped into categories.  Any command without a "dot" is assumed to be a member of the "Core" category.

By default, a command changes the active text and passes it to the next command. However, the result of a command can be instead redirected into variable stored and stored for later use.  This does _not_ change the active text.

You can direct the result of an operation to a variable by using the "=>" operator and a variable name at the end of a statement.

Here's an example of chaining filters and writing into and out of variables to obtain and format the temperature in Sioux Falls:

    Http.Get http://api.openweathermap.org/data/2.5/weather?q=Sioux+Falls&mode=xml&units=imperial
    Xml.Extract //city/@name => city
    Xml.Extract //temperature/@value => temp
    Format "The temp in {city} is {temp}."
    Html.Wrap p weather-data

    # <p class="weather-data">The temp in Sioux Falls is 37.</p>

Variables are volatile -- writing to the same variable multiple times simply resets the value each time.

Filters are pluggable. Simply write a static class and method, like this:

    [TextFilters("Text")]
    public static class TextFilters
    {
      [TextFilter("Left")]
      public static string Left(string input, TextFilterCommand command)
      {
        var length = int.Parse(command.CommandArgs[0]);
        return input.Substring(0, length);
      }
    }

Then register this with the pipeline:

    TextFilterPipeline.AddType(typeof(TextFilters));

This command is now available as:

    Text.Left 10

In this case, we're trusting that this will be called with (1)at least one argument (any extra arguments are simply ignored), (2) that will parse to an Int32, and (3) isn't longer than the active text.  Clearly, you're gonna want to validate and error check this before doing anything.

You can map the same filter to multiple command names, then use that name inside the method to do different things.

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

## History

This started out as a simple project to include the contents of a text file in EPiServer.  That CMS provides "blocks," which are reusable content elements.

I wrote a simple block into which a editor could specify the path to a file on the file system. The block would read the contents of the file and dump it into the page.  It was essentially a server-side file include for content editors.

Then I got to thinking that some files might need to have newlines replaced with BR tags, and how would I specify that?  And what if the file wasn't local?  How would I specify a remote file?  And what if it was XML -- could I specify a transform?

And the idea of a text filter pipeline was born.  To support this, I needed to come up with language constructs, and that's when I started parsing commands. And then I found I could enable some neat functionality by tweaking and tuning and making small changes.

And when the snowball finally came to a rest at the bottom of the hill, you had, well, this.

The constant challenge with this type of project is when to stop. At what point are you simply inventing a new programming language.  When do you cross the line from simple and useful to pointless and redundant?

I don't have an answer for that (hell, I may have crossed that line already).  I leave it to you to judge.

Happy filtering.

Deane Barker
January 2015
