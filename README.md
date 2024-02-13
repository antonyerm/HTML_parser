# Templater library
_Author - Anton Yermolayev (https://github.com/antonyerm/)_
This is a solutions for the test task:
"You need to build a library that will be able to generate HTML based on template and JSON data."

**Folders:**  

_Client_ - a console app which loads the data from the files in _TestData_ folder.
_Templater_ - the library with CreateHtml method.
_TemplateTests_ - a test project with some tests.

**Usage:**  

Put your data in _DataFile.json_ and your template in _TemplateFile.html_. Start the _Client_ project. The result will be printed on screen and saved to _output.html_ file. The additional parameters (default data) is hardcoded in `Constants` class.

**Highlights**:
1. The solution implements a `for` loop. However it can be scaled to support multiple operators, as it uses the Factory pattern.
1. The solution has validation code which throws exceptions when errors in a template or input data occur.
1. The solution has unit tests for valid cases and invalid input files.

**Algorithm**:
1. The method deserializes input JSON data and parses the template using HtmlAgilityPack.
1. It creates a list of all nodes which have texts with operator tags inside and associates an operator delegate for each of them.
1. It starts the delegate for each of the nodes. The delegate replaces the child nodes with processed ones.

**Possible improvements**:
1. Probably we could do well without parsing the template with HtmlAgilityPack. The operators are applied on line level, so we don't need to work with individual nodes. This could result in a simpler solution.
1. We could use configuration manager instead of hardcoded constants for paths and default data.



