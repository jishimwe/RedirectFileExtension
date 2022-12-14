# Remote Development and Compilation

___

**Author** : *Ishimwe Jean-Paul*

**Noma** : *6919-12-00*
___

## Redirect File Extension

This project is an extension for **Visual Studio**. It handles *Redirect* projects by implementing functions to download and upload files via their *redirect* equivalent.

### Implmented functions

- Configuration of the extension: \
A simple function that can be used to configure the extension by giving necessary informations to use the *Redirect* as well as the *Real* repository.

![Configuration](./Screenshots/Config.png)


- Opening a file: \
This function will open a file in Visual Studio via its *redirect* counterpart. It will download the *real* file if it's locally unavailable.

![Open File from Redirect](./Screenshots/OpenFile.png)


- Uploading a file: \
This function will upload a file to the repository. It requires a message. If the file is not already in the *repository*, then you need to import it first.

![Uploading a File](./Screenshots/Upload.png)


- Forcing the upload of a file: \
Same as uploading a file, but conflicts will be ignored.


- Updating the project: \
Update the project with the changes made remotely.


- Solving conflicts: \
In case of conflict, this function solve them by simple mechanism. It can either create an union of the remote and local changes, accept local changes, accept remote changes or create a file with conflicts markers.

![Merge options](./Screenshots/Merge.png)


- Importing a file: \
Add a file to the repository. If the file is already in the real repository, it will be added with its corresponding relative path. If it's not, it will be added to the root of the real repository.

![Import](./Screenshots/Import.png)