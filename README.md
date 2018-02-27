# CheckPwnedPasswords
Checks to see if a password is in the list of 300+ million passwords downloaded from https://haveibeenpwned.com/Passwords

## How to use

Launch from the command line. This is a .NET Core executable, so you will need to install .NET Core and run

    > dotnet CheckPwnedPasswords.dll

You will need to specify a set of files to use as password hash dictionaries. To do that, just list them as command-line parameters. You may also list directories: in that case, all the files in the directory will be used as dictionaries. For example, 

    > dotnet CheckPwnedPasswords.dll C:\Pwd\passwords.txt C:\Pwd\MorePasswords

will use both the `C:\Pwd\passwords.txt` file and all the files in `C:\Pwd\MorePasswords` (and its subdirectories) as dictionaries. 

A default directory is added, if it exists: the `data` subfolder in the working directory. 