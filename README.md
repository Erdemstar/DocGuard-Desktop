# DocGuard-Desktop

## The parameters it takes
* **credentials**		: true if user information is entered otherwise false
* **email**				: Email address of user
* **password**			: Password of user
* **sourceFile**		: Location of file to send
* **sourceFolder**		: Location of files to send
* **destinationUrl**	: DocGuard file analysis address
* **outputPath**		: Location where the obtained outputs will be written
* **threadSleep**		: Expected time between operations in seconds

## How to use

### Credentials : false 
* DocGuard-Desktop.exe --credentials false --sourceFolder C:\Testuser\Arge\Office\Mix --destinationUrl https://api.docguard.net:8443/ --outputPath C:\Users\Testuser\source\repos\DocGuard-Desktop\DocGuard-Desktop\ --threadSleep 2 
* DocGuard-Desktop.exe --credentials false --sourceFile C:\Testuser\Arge\Office\Mix\serversidetemplateinjection.pdf --destinationUrl https://api.docguard.net:8443/ --outputPath C:\Users\Testuser\source\repos\DocGuard-Desktop\DocGuard-Desktop\ --threadSleep 2 

### Credentials :  true 
* DocGuard-Desktop.exe --credentials true --email Testuser@streamer.com --password streAmer* --sourceFolder C:\Testuser\Arge\Office\Mix  --destinationUrl https://api.docguard.net:8443/ --outputPath C:\Users\Testuser\source\repos\DocGuard-Desktop\DocGuard-Desktop\ --threadSleep 2 
* DocGuard-Desktop.exe --credentials true --email Testuser@streamer.com --password streAmer* --sourceFile C:\Testuser\Arge\Office\Mix\serversidetemplateinjection.pdf  --destinationUrl https://api.docguard.net:8443/ --outputPath C:\Users\Testuser\source\repos\DocGuard-Desktop\DocGuard-Desktop\ --threadSleep 2 

## Example Output
For sample output, created a file named sample_output.json. Before a scan, you can control how you get the output.
