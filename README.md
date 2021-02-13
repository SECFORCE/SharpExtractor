
# About

SharpExtractor is a dotnet project that allows file extraction from file containers. 

The initial version allows extraction from vhdx files, but has everything needed to extract from other type of containers suchs as zip, vmdk, vdi, vhd, or any other suported by the [DiscUtils](https://github.com/DiscUtils/DiscUtils) library.

Core code is a shamefull copy of code from [RecursiveExtractor](https://github.com/microsoft/RecursiveExtractor). 

The motivation of this project is to have a dotnet framework (as opposed to dotnet core) library capable of being executed from Cobalt Strike, which means meeting the 1mb maximum size constraint.

Obviously, this assembly is compatible with Cobalt Strike.

# Suported File Types

vhdx

(more on the way)

# Usage

Usage is pretty straighforward.

~~~
a:allow-filter          Optional regex allow filter
d:deny-filter           Optional regex deny filter
e:extractor             The extractor to use (vhxd)
f:file                  The file to process
i:ignore-filters        Forces extraction when no filters are specfied
o:output-dir            Output directory
~~~

Deny filters are evaluated first. 
The ignore filters flag allows to force full extraction. It is a protection to avoid uninteded extraction of all files.
If extracted file already exists on the output directory, it will be renamed to <filename>_n.<extension>.

Extract ntds.dit and system from a vhdx backup file:
~~~
SharpExtractor -e vhdx -a (?i)ntds.dit (?i)system  -f f:\backups\20201225.vhdx -o c:\programdata\ 
~~~

