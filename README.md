# MPF Snippet Creator
Utility to create Visual Studio code snippets for Mission Pinball Framework YAML definitions.

## Details
This console application will take the Mission Pinball Framework (MPF) yaml configuration file (config_spec.yaml) and create an output file (snippets.txt) containing snippet JSON definitions that can be imported into Visual Studio Code and other platforms.

## How to use the application
Running the application, you can specify a command line argument for the directory to find the config_spec.yaml file.  If this is left blank, the program will look in your current directory.  Once the application completes successfully, you will have a snippets.txt file created in the same directory that can be imported into your IDE.
