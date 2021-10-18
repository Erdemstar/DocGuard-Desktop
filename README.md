# DocGuard-Desktop

## The parameters it takes
**--sourceFolder** : Location of files to send
**--destinationUrl** : DocGuard file analysis address
**--outputPath** : Location where the obtained outputs will be written
**--threadSleep** : Expected time between operations in seconds
## How to use
DocGuard-Desktop.exe --sourceFolder C:\TestPC\Arge\Office\Mix  --destinationUrl https://api.docguard.net:8443/api/FileAnalyzing/AnalyzeFile/  --outputPath C:\Users\TestPC\source\repos\DocGuard-Desktop\DocGuard-Desktop\ --threadSleep 2

## Example Output
For sample output, I created a file named sample_output.txt. Before a scan, you can control how you get the output.