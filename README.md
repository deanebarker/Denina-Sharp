# Text Filtering Pipeline

TFP is a pipeline processor for text, intended for editorial usage through configuration by simple text commands. A pipeline is a series of filters, processed in sequential order.  In most cases, the output from one filter is the input to the filter immediately following it (this is the "active text").

Say we have the string "BAR" and we'd like to prepend FOO to it.  We create a pipeline, and add a command to invoke the "Prepend" filter, passing it "FOO" as the first argument.

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

Clearly, this is verbose.  So commands can be added by simple text strings.  The strings are tokenized on whitespace. The first token is the command name, the subsequent tokens are arguments. (Any arguments which contain whitespace need to be in quotes.)

    var pipeline = new TextFilterPipeline();
    pipeline.AddCommand("Prepend FOO")
    var result = pipeline.Execute("BAR");

The result will be "FOOBAR".

The pipeline remains "loaded" with commands, so we could just as easily do this immediately after:

    pipeline.Execute("BAZ");

We'd get "FOOBAZ."

"Prepend" is one example of several dozen commands. Some take arguments, some don't. It's up to the individual commands how many arguments they need, what order they need them in, and what they do with them (much like function calls in any programming language).

The pipeline doesn't have to start with text, as some commands can allow the pipeline to acquire text mid-stream.  For example, the command configuration to call the home page of Gadgetopia and extract the title looks like this:

    Http.Get gadgetopia.com
    Html.Extract //title

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
