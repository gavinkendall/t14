﻿T14 Interpreted Scripting Language by Gavin Kendall
[The information presented in this README refers to version 1.0.0.3]
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
written in the provided T14 script. The output should be ...

Well hello there, General Kenobi.
Perhaps it's also a good time to say, "Well hello there Jedi Master Luke!"
- Star Wars Episode III -

You can also run the T14 interpreter with a T14 method and get the output
in your terminal. For example ...
t14.exe ::dec->bin[65]
... will output ...
1000001

If you need to use whitespace then make sure to include double-quotes in
the method being passed to the interpreter. For example ...
t14.exe "::text->morse[hello world]"
... will output ...
.... . .-.. .-.. ---/.-- --- .-. .-.. -..



Writing Your First T14 Script
-----------------------------
The T14 language is, structurally, based on blocks of code. You start a block
with ::start and end it with ::end and every T14 script needs, at least,
a block named "main". So open your text editor, write the following lines in a
new file and save the file as "hello.t14" (or whatever filename you choose) ...
::start[main]
hello world
::end

... then go to your terminal and run the T14 interpreter against "hello.t14"
with the command "dotnet t14.dll hello.t14" and the script will output ...
hello world



Blocks
------
Did it work? If so let's continue with writing another block in your script by
adding ::start[my_block] and ending it with ::end and then we'll run a block
with the ::run command so your T14 script will now look like this ...
::start[main]
hello world
::run[my_block]
::end
::start[my_block]
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
::start[main]
::set [hello] = hello world
I'm just here to say [hello].
::end

You can change a value of a variable by setting a different value ...
::start[main]
::set [hello] = hello
[hello]
::set [hello] = hi
[hello]
::end

You can also replace the value of a variable with the value of another variable
like this ...
::start[main]
::set [black] = black
::set [pink] = pink
::set [black] = [pink]
[black]
::end



Making Comparisons
------------------
You can check the equality of two values with the ::if command and run a block
if the equality is true or run a block if the equality is false.

For example ...
::if [7 == 7]->[run_this_block]
::if [7 == 7]->[run_this_block_if_equal] else [run_this_block_if_not_equal]
::if [7 > 3]->[run_if_true] else [run_if_false]

The following operators can be used.
==
!=
<
>
<=
>=

You can also use a variable with the ::if command.

For example ...
::if [[my_variable] == hello]->[run_this_block_if_equal]



Getting Input
-------------
You can get the input from the console by using the ::input command.
This command requires text to display for output and a variable.

For example, to ask for the user's name and then display their name ...
::input [What's your name? ]->[name]
You said your name is [name].

You could display a command prompt ("> ") and get the input for the
command then decide what to do. Here's a complete T14 script ...

::start[main]
::input [> ]->[command_from_prompt]
::if [[command_from_prompt] == ask for name]->[do_something]
::end

::start[do_something]
::input [What's your name? ]->[name]
Hello [name]!
::end





Conversion Methods
------------------
Here is a list of methods you can use in your T14 script to convert stuff.
::dec->bin[65]
::dec->hex[65]
::dec->ascii[65]
::bin->dec[01000001]
::bin->hex[01000001]
::bin->ascii[01000001]
::hex->bin[41]
::hex->dec[41]
::hex->ascii[41]
::ascii->bin[A]
::ascii->hex[A]
::ascii->dec[A]
::text->morse[hello world] or ::morse[hello world]
::morse->text[.... . .-.. .-..---/.---- - .-. .- ..-..]
::dec->roman[13]
::roman->dec[XIII]