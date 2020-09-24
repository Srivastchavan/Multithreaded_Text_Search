# Multithreaded_Text_Search
Multithreaded text search windows forms app in C#

This program searches for a string in a text file using multiple threads.  It finds all occurrences of the string and shows them in a list.
1.	Shows a textbox for uploading of a text file. File is uploaded using the Browse button which brings up a FileOpenDialog box.
2.	Another textbox accepts text to be found in the document.
3.	A search button is used to search the required text in the uploaded file.
4.	A ListView control displays the entire line on which the text was found and the line number within the document. 
5.	Case-insensitive comparison is used to search each line of file with entered text. 
6.	The Search button text is changed to “Cancel” once the search starts, and cancels the operation when pressed.
