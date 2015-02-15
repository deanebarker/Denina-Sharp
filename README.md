# Denina Sharp: Denina in <span>C#</span>

Denina is a simple text processing language which allows a non-technical editor to configure and execute sequential C# methods on text.

[Learn more about Denina](http://denina.org)

Denina Sharp is a C# implementation of the Denina language.

## Working with Denina Sharp

(Note: The words "command" and "filter" get used interchangeably in this document. Technically, a "command" is an object that invokes and configures a "filter," which is a method. In practice, I'll go back and forth between the terms indiscriminately. Sorry.)

Here's the C# to instantiate the pipeline and add a command, the long way.

    var pipeline = new Pipeline();
    pipeline.AddCommand(
       new PipelineCommand()
       {
         CommandName = "Text.Prepend",
         CommandArgs = new Dictionary<object,string>() { { 1,"FOO" } }
       }
      );
    var result = pipeline.Execute("BAR");
    // Result contains "FOOBAR"

Clearly, this is _way_ too verbose, and it provides no simplified proxy through which an editor might work with Denina within their CMS.

To make it more concise and editor-friendly, commands can be added via simple text strings.  The strings are tokenized on whitespace. The first token is the command name, the subsequent tokens are arguments. (Any arguments which contain whitespace need to be in quotes.)

    var pipeline = new Pipeline();
    pipeline.AddCommand("Text.Prepend FOO"); be passed into the constructor
    var result = pipeline.Execute("BAR");

The result will be "FOOBAR".

We can shorten it even more by passed commands into the constructor:

    var pipeline = new Pipeline("Text.Prepend FOO");

In fact, commands can be passed in _en masse_, separated by line breaks (note that command parsing is broken out to its own class, and could easily be re-implemented, if you wanted to do something different).  Each line is parsed as a separate command.

    var pipeline = new Pipeline(thousandsAndThousandsOfCommands);

The pipeline remains "loaded" with commands even after execution, so we could just as easily do this immediately after:

    pipeline.Execute("BAZ");

We'd get "FOOBAZ."  We could pass a thousand different strings to the pipeline, and they would all come out with "FOO" prepended to them.

"Prepend" is one example of several dozen pre-built filters. Some take arguments, some don't. It's up to the individual filter how many arguments it needs, what order it needs them in, and what it does with them during execution (much like function calls in any programming language).

The pipeline doesn't have to start with text, as some filters allow the pipeline to acquire text mid-stream. In these cases, the pipeline is invoked without arguments.

    pipeline.Execute();
    
Finally, the pipeline can be "pre-primed" with variables prior to execution, from the C# instantiation code.  For example:

    var pipeline = new Pipeline();
    pipeline.SetVariable("searchQuery", Request.Querystring["q"]);

In this case, $searchQuery will be available to commands, like this:

    Text.Format "SELECT * FROM Something WHERE myField = '{searchQuery}'"

Obviously, the basic rules of input sanitizing still apply -- you're going to want to sanitize the value in C# before setting it.

## Writing Filters

Filters are pluggable. Simply write a method, like this:

      public static string Left(string input, PipelineCommand command)
      {
        var length = int.Parse(command.CommandArgs[0]);
        return input.Substring(0, length);
      }

The method needs to take in two arguments:

1. **String:** this is the input; what is passed to the filter
2. **PipelineCommand:** this is an object representation of the line of text used to call the filter. On it are properties to access the command name and the arguments.

The method does whatever it wants to the input string, and returns the result as another string.  The method doesn't need to worry about writing into or out of variables -- those actions are handled by the pipeline itself.

Then register the MethodInfo object with the pipeline, telling it the category and name of the filter.

	var method = typeOf(MyClass).GetMethod("Left");
    Pipeline.AddMethod(method, "Text", "Left");

After registering, your command is now available as:

    Text.Left 10

The category and name arguments are optional. If you include the Filter attribute on method declaration, you don't have to pass that in:

    [TextFilter("Left")]

Alternately, you can register an entire type full of methods:

    Pipeline.AddType(typeof(MyFilters), "Text")

That will search the type for all methods with a Filter attribute.  Like with methods, you can identify the category with an a Filters (note the plural) attribute on the class, and you don't have to pass the category name in:

    [TextFilters("Text")]

Then register like this:

    Pipeline.AddType(typeof(MyFilters)

Finally, you can even register entire assemblies at a time, if all your filters are in a separate DLL:

    var myAssembly = Assembly.LoadFile(@"C:\MyFilters.dll");
    Pipeline.AddAssembly(myAssembly);

All the types in that assembly marked with the Filters attribute will be searched for methods with the Filter attribute.  (In fact, when the Pipeline class first initializes, it loads its built-in filters by simply passing the currently executing assembly to AddAssembly.)

Note that the name of the underlying C# method is irrelevant.  The filter maps to the combination of the category name ("Text," in this case) and filter ("Left"), both supplied by the attributes. While it would make sense to call the method the same name as the filter, this isn't required.

In the example above case, we're trusting that this filter will be called with (1) at least one argument (any extra arguments are simply ignored), (2) that the argument will parse to an Int32, and (3) that the numeric value isn't longer than the active text.  Clearly, _you're gonna want to validate and error check this inside your filter before doing anything_.

And what happens if there's an error condition?  Do you return the string unchanged?  Do you throw an exception?  That's up to you, but there is no user interaction during pipeline execution, so error conditions are problematic.

You can map the same filter to multiple command names, then use that name inside the method to change execution.

    [Filters("Text")]
    public static class TextFilters
    {
      [Filter("Left")]
      [Filter("Right")]
      public static string Left(string input, PipelineCommand command)
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

1. The source to create an assembly named "DeninaSharp.dll"
2. A test project with moderate unit test coverage (needs to be better, clearly)
3. A WinForms testing app (Denina.exe) which provides a GUI to create, execute, and test filters.

On build, the DLL, a supporting DLL (HtmlAgilityPack), and the WinForms EXE are copied into the "Binaries" folder. The WinForms tester should run directly from there.
