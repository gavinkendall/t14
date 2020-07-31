T14 Interpreted Scripting Language by Gavin Kendall
Last updated on 2020-07-31 (July 31, 2020)
[The information presented here refers to version 1.0.0.1]
================================================================================



Summary
-------
T14 is a language for text-based console applications.

You will need to install .NET Core Runtime from https://dotnet.microsoft.com/
in order to successfully run the T14 language interpreter on your system.

You will also need these files from https://sourceforge.net/projects/t14/files/
t14.dll
t14.exe
t14.runtimeconfig.json

Optionally, you can download the provided "hello.t14" file to experiment
writing and running a T14 script.

You would normally run the T14 interpreter with a given filename that ends
with a ".t14" file extension as the first command line argument for the
interpreter. For example, on Windows, you would run "t14.exe hello.t14" and
on a Mac or Linux system with Microsoft .NET Core you would run
"dotnet t14.dll hello.t14" to interpret, and parse, the T14 scripting language
written in the provided T14 script.



Writing Your First T14 Script
-----------------------------
The T14 language is, structurally, based on blocks of code. You start a block
with ::start and end it with ::end and every T14 script needs, at least,
a block named "main". So open your text editor, write the following lines in a
new file and save the file as "hello.t14" (or whatever filename you choose) ...
::start(main)
hello world
::end

... then go to your terminal and run the T14 interpreter against "hello.t14"
with the command "dotnet t14.dll hello.t14" and the script will output ...
hello world



Blocks
------
Did it work? If so let's continue with writing another block in your script by
adding ::start(my_block) and ending it with ::end and then we'll run a block
with the ::run command so your T14 script will now look like this ...
::start(main)
hello world
::run(my_block)
::end
::start(my_block)
hello again
::end

... this will output ...
hello world
hello again

You can write as many blocks as you want and each block can run the commands
between a block's ::start and ::end commands with the ::run command.



Variables
---------
To define a variable use the ::set command and then you can output the value of
the variable within a block. For example, in your main block ...
::start(main)
::set $hello = hello world
I'm just here to say $hello
::end

... this will output ...
I'm just here to say hello world



Conversion Methods
------------------
Here is a list of methods you can use in your T14 script to convert stuff.
::dec->bin(65)
::dec->hex(65)
::dec->ascii(65)
::bin->dec(01000001)
::bin->hex(01000001)
::bin->ascii(01000001)
::hex->bin(41)
::hex->dec(41)
::hex->ascii(41)
::ascii->bin(A)
::ascii->hex(A)
::ascii->dec(A)
::text->morse(hello world) or ::morse(hello world)
::morse->text(.... . .-.. .-..---/.---- - .-. .- ..-..)