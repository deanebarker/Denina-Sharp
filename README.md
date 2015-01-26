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

Using "WriteTo" and "ReadFrom," commands can read and write values into arbitrary variables. Reading from a variable resets the active text to the value of that variable, destroying the previously active text.

An example of chaining filters and writing into and out of variables to obtain and format the temperature in Sioux Falls:

    # Get the XML source and write it to the variable called "xml" so we can get it back when we need to
    Http.Get http://api.openweathermap.org/data/2.5/weather?q=Sioux+Falls&mode=xml&units=imperial
    WriteTo xml

    # Extract the city name (making it the active text) and then write that to a variable called "city"
    Xml.Extract //city/@name
    WriteTo city

    # Read back the original XML
    ReadFrom xml

    # Extract the temperature (making it the active text) and then write that to a variable called "temp"
    Xml.Extract //temperature/@value
    WriteTo temp

    # Format both into a sentence and wrap it in an HTML tag with a class name
    Format "The temp in {city} is {temp}."
    Html.Wrap p weather-data

    # The result is:
    # <p class="weather-data">The temp in Sioux Falls is 37.</p>

WriteTo stores the active text in the specified variable, _but doesn't change it_ (meaning the active text is still available by the next command). ReadFrom immediately changes the active text to the value of the specified variable.
