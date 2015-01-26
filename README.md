# Text Filtering Pipeline

TFP is a pipeline processor for text, intended for editorial usage through configuration by simple text commands. A pipeline is a series of filters, processed in sequential order.  In most cases, the output from one filter is the input to the filter immediately following it (this is the "active text").

An example of the C# to invoke the pipeline to append "FOO" to incoming text.

    var pipeline = new TextFilterPipeline();
    pipeline.AddCommand(
       new TextFilterCommand()
       {
         CommandName = "Prepend",
         CommandArgs = new Dictionary<object,string>() { { 1,"FOO" } }
       }
      );
    var result = pipeline.Execute("BAR");
    // Result contiains "FOOBAR"

Clearly, this is verbose.  So commands can be added by simple text strings.  The strings are tokenized on whitespace. The first token is the command name, the subsequent tokens are arguments.

    var pipeline = new TextFilterPipeline();
    pipeline.AddCommand("Prepend FOO")
    var result = pipeline.Execute("BAR");

It's up to the individual commands how many arguments they need and what they do with them.

The pipeline doesn't have to start with text, as some commands can allow the pipeline to acquire text.  For example, the command configuration to call the home page of Gadgetopia and extract the title looks like this:

    Http.Get gadgetopia.com
    Html.Extract //title

Commands are grouped into categories.  Any command without a "dot" is assumed to be a member of the "Core" category.

By default, a command changes the active text and passes it to the next command. However, the result of a command can be instead written into a variable and stored for later use.  This does _not_ change the active text.

You can direct the result of an operation to a variable by using the "=>" operator and a variable name at the end of a statement.

Here's an example of chaining filters and writing into and out of variables to obtain and format the temperature in Sioux Falls:

    Http.Get http://api.openweathermap.org/data/2.5/weather?q=Sioux+Falls&mode=xml&units=imperial
    Xml.Extract //city/@name => city
    Xml.Extract //temperature/@value => temp
    Format "The temp in {city} is {temp}."
    Html.Wrap p weather-data

    # The result is:
    # <p class="weather-data">The temp in Sioux Falls is 37.</p>

Variables are volatile -- writing to the same variable multiple times simply resets the value each time.
