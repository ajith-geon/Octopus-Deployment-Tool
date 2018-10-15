## Welcome to the WinForm-Application-for-Octopus-Deployment wiki!
## Pending Updates, New Relese Version_4.0.1 Released with more customized features

### 1. Application UI Preview
![](https://github.com/AjithGeorge/WinForm-Application-for-Octopus-Deployment/blob/master/UI%20Preview.png?raw=true)

***
### 2. Different Elements Explained

* **API KEY** - Used for Authentication Purpose [ "_Remember Me_" option saves the data in user configuration file ] <br>[Generate Octopus API Key](https://github.com/AjithGeorge/WinForm-Application-for-Octopus-Deployment/wiki/Octopus-API-Key-Generation)
* **Channels** - **Populate Channels** will fetch all the available channels for the project.<br/> More Details on Channels available at [Octopus Channels](https://octopus.com/docs/deployment-process/channels)
* **Package Version** - Specify the Version of the Package to be used for creating Release Example: 8.65.3.1-pr1001
* **Release Name** - Specify Name of the Release. Example: 8.65.3.1-pr1001-V1 <br> The Package Version and Release Name are to filled in case if user need to create a new release.
* **Create Release** - A release will be created with the given Package Version and Release Name.
* **Release/Populate Release** - Populate Release button will fetch all the available releases created in the specified channel. <br>The Required Release is to be selected for deploying.
* **Environments/Populate Environments** - The populate Environments button will fetch all the available environments for deployment process.
* **Deploy Release** - This will trigger the deployment of the selected Release into the specified Environment. <br>On Successful triggering of Deployment, the Octopus dashboard will be displayed in the default browser automatically.
* **Rich Text Box** - This Serves as a Console for displaying the progress/success message and alerts/warnings if any. 
***
### 3. Advanced Deployment
* **Steps to be skipped** - Ticked Steps will be skipped in the deployment process.
<br>The Step details (names and values) are hard-coded (as they don't change often and improved performance is obtained than dynamic loading of the same). Details are provided in the Code Walk-though Page. 
* **Deployment Variables/Parameters** - These are the custom variables set in octopus for facilitating deployment process.
<br>(_The Text-boxes corresponding to the labels: "Block on Data..."_)
<br>These Variables should be according to your deployment process. <br>Provided is only an example from a project.
More details available at [Octopus Variables.](https://octopus.com/docs/deployment-process/variables)







### Application is using API Key for Authentication instead of Username/Password.
***

The steps to create Octopus API key is available at : [How to generate Octopus API key?](https://octopus.com/docs/api-and-integration/api/how-to-create-an-api-key)


### Listed are the Items that can be made custom depending on requirement.
***

*  **Steps to be Skipped** : 
<br> Go to WindowsAppForOctopusDeployment > Form1.cs.
<br> Locate the Code block.
![](https://github.com/AjithGeorge/WinForm-Application-for-Octopus-Deployment/blob/master/RefPics/SkipItemsCodeRef.png?raw=true)
<br> Change the Step-Name and Value to required Ones. The Values can be obtained by monitoring the POST data content of the request by any Network Analyzing tool. [Chrome/Firefox inbuilt Network Monitor will suffice]

***

* **POST Request JSON Templates**:
<br> Templates are used for the POST request JSON body content.
<br> The Templates can be fetched dynamically, but as the template hardly changes the dynamic loading of the same will be an overkill. Keeping the templates locally adds to the performance and easy manipulation of data is possible.
<br>If there happens any change in any Properties/Steps [addition/deletion], updating the same in the local template will be enough. 
![](https://github.com/AjithGeorge/WinForm-Application-for-Octopus-Deployment/blob/master/RefPics/POST%20Content%20Template.png?raw=true)
<br>These templates have the structure of the JSON body required for the API request but don't have the values. [Only Properties are defined in the template, values are entered through the code]

***
* **Parsing JSON**:
<br>Locate the code block
![](https://github.com/AjithGeorge/WinForm-Application-for-Octopus-Deployment/blob/master/RefPics/Create%20Release%20Post%20Data%20Input.png?raw=true)
<br>This is the step where the Json template is taken as input and the values are inserted.
<br>Here the user input values are assigned to the Json Property Values.
![](https://github.com/AjithGeorge/WinForm-Application-for-Octopus-Deployment/blob/master/RefPics/Create%20Release%20POST%20Json%20Content%20Manipulation.png?raw=true) 
The final Json with the user given values and specifics are saved as a new file _"CreateReleasePostData.Json"_. 
<br>The contents of this file is converted to string and is passed as the POST method body.
<br>The Same procedure is used for the Deploy Release POST data too.
