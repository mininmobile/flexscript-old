# FlexScript
An old programming language project I want to revisit

## Syntax
### Commands/Functions
`command` will be the command executed, `argument1`, `argument2` and `etc` will be parsed as arguments
```bash
command argument1 argument2 etc
```

### Comments
FlexScript uses `//` for comments, anything after `//` will be ignored.

### Variables
To define and interact with variables, the command is `var`, you do not need to specify types.

### Types
In FlexScript, everything is stored as a string, unless it needs to be converted to another type, in that case it will be temporarily converted, and stored back as a string, see `Math` under `Examples`.

## Examples
### Hello World
```bash
print Hello, World! // output is 'Hello, World!'
```

### Pause
```bash
print I hope you have a nice day

pause // Outputs 'Press any key to continue...' and waits for user keypresss

pause -s // Applying arguments -s or --silent outputs nothing and waits for user keypress

pause -c Press any key... // Applying arguments -c or --custom outputs custom text (`Press any key...` in this case) and waits for user keypress

print SIKE!
```

### Colors
```bash
background white // Set background color to white
color black // Set foreground color to black

clear // Clear the terminal window, applying colors
```

### Variables
```bash
var foo = bar
print foo{foo} // output is 'foobar'
```

### User Input
```bash
var input <= Enter something: // Gets user input, outputs 'Enter something:'
var input2 < // Gets user input, outputs '' (nothing)

print {input}
print {input2} // Outputs user content
```

### Math
```bash
var a += 5 5 // a is '10'

var b -= 20 5 5 // b is '10'

var c *= 4 2 8 8 // c is '512'

var d = {c} // d is '512', value of c
var d /= 8 // d is '64'
```

### If Statements
```bash
var x = 5
var y = 5
var foo = bar
var bar = foo

if {x} == {y} then print match! // output is 'match!'
if {foo} != {bar} then print !match // output is '!match'
```
