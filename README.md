# FlexScript
An old programming language project I want to revisit

## Syntax
### Commands/Functions
`command` will be the command executed, `argument1`, `argument2` and `etc` will be parsed as arguments
```bash
command argument1 argument2 etc
```

### Comments
FlexScript uses `//` and `#` for comments, anything after `//` or `#` will be ignored.

### Variables
To define and interact with variables, the command is `var`, you do not need to specify types.

### Loops
There are 2 types of loops, `for` and `while`, `for` is like a swiss army knife and `while` is generic. (while is not implemented)

### Labels
You can define a label on a newline with `:labelname` where `labelname` is the name of your label, then you can jump to it using the `goto` command, see `'While' Loop` under `Examples`.

### Types
In FlexScript, everything is stored as a string, unless it needs to be converted to another type, in that case it will be temporarily converted, and stored back as a string, see `Math` and `Arrays` under `Examples`.

## Examples
### Hello World
```bash
print Hello, World! # output is 'Hello, World!'
```

### Pause
```bash
print I hope you have a nice day

pause # Outputs 'Press any key to continue...' and waits for user keypresss

pause -s # Applying arguments -s or --silent outputs nothing and waits for user keypress

pause -c Press any key... # Applying arguments -c or --custom outputs custom text (`Press any key...` in this case) and waits for user keypress

print SIKE!
```

### Colors
```bash
background white # Set background color to white
color black # Set foreground color to black

clear # Clear the terminal window, applying colors
```

### Variables
```bash
var foo = bar
print foo{foo} # output is 'foobar'
```

### User Input
```bash
var input <= Enter something: # Gets user input, outputs 'Enter something:'
var input2 < # Gets user input, outputs '' (nothing)

print {input}
print {input2} # Outputs user content
```

### Math
```bash
var a += 5 5 # a is '10'

var b -= 20 5 5 # b is '10'

var c *= 4 2 8 8 # c is '512'

var d = {c} # d is '512', value of c
var d /= 8 # d is '64'
```

### Arrays
```bash
var lines =[] hello,world,foo,bar

print {lines} # outputs 'hello,world,foo,bar'
print {lines[0]} # outputs 'hello'
```

### If Statements
```bash
var x = 5
var y = 5

if {x} == {y} then print match! # Output is 'match!'
if {x} != {y} then print !match # Outputs nothing
if {x} > {y} then print bigger than # Outputs nothing
if {x} >= {y} then print bigger than or equal to # Outputs 'bigger than or equal to'
```

### Try Catch
```bash
# cause an error and output it to the screen
# outputs 'AN ERROR HAS HAPPENED: Invalid Token; variable doesnt exist'
try print {i} catch print AN ERROR HAS HAPPENED: {e}
```

### 'While Loop'
```bash
var i = 0 # create index
:loop
var i += 1 # increment index

print {i} # print index

if {i} < 5 then goto loop # while index is less than 5, loop
```

### For/times Loop
```bash
for i times 10 do print {i} # Outputs numbers '1' through '10'
```

### For/in Loop
```bash
var array = apple,banana,orange,strawberry # =[] is redundant for this use case as were not referencing specific items with array[x]

for item in {array} then print {item} # prints current item from array
```

### "Classes" and "Instancing"
```bash
var foo = bar

var myclass = class foo:{foo},bar:foo
var myinstance =: new {myclass}

print {myinstance.foo} # prints 'bar'
```

### Directories
```bash
# create a new directory

directory new my folder

# delete a directory

directory delete my folder
```

### Files
```bash
# get contents of a file, seperate lines are split by commas, this creates a few variables:
# 
# file          : placeholder file variable
# file.lines    : array of lines in file
# file.lines[x] : get specific line from lines array
# file.length   : get amount of lines in file

file get myfile.txt

# delete a file

file delete myfile.txt

# check if file exists, creates 
# 
# file        : placeholder file variable
# file.exists : true or false, does referenced file exist

file exists file.txt

# override a file with a line or lines, foo.txt after this command:
# 
# hello
# world

file write hello,world to foo.txt

# append a line or lines to end of file, bar.txt after this command:
# 
# this line was already in this file
# me too!
# but not me

file append but not me to bar.txt
```
