file exists debug.txt
if {file.exists} == true then goto yes

:no
	print file doesn't exist
	:(
goto eof

:yes
	file get debug.txt
	for line in {file.lines} then print {line}
goto eof

:eof
pause -s
