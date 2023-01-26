# File Comparer

Just a simple file/directory comparer.

It is only used to compare whether the contents of files or folders are different, not to compare the specific differences.

MD5 is used for comparison. False negatives may occur, which can be eliminated by removing `MD5.ComputeHash()` function call.

## How to use

Select two files or folders, and drag them to the EXE.
