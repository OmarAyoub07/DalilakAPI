
Welcome to RavenDB Cloud!

In order to connect to your instance, please install one of the supplied client certificates using the guide below.

Windows Chrome users
====================
Double click the .PFX file (the one without a password) and add it to the OS store. There are no special settings and you may click next all the way through.
Now you can access RavenDB by typing the URL in Chrome. You should be able to pick your newly installed certificate from the list of available certificates.

* If Chrome doesn't let you choose a certificate and instead you get a RavenDB authentication error, please try in an incognito window (or close all instances of Chrome and try again). It can happen because the browser caches the client certificates.

Linux Chrome users
====================
In Chrome, go to Settings -> Manage Certificates
Import the password protected .PFX file under "Your Certificates" tab (the password can be found in a separate file called PASSWORD.TXT).
Now you can access RavenDB by typing the URL in Chrome.

* If Chrome doesn't let you choose a certificate and instead you get a RavenDB authentication error, please try in an incognito window (or close all instances of Chrome and try again). This can happen because the browser caches the client certificates.

Firefox users
===================== 
In Firefox, go to Options -> Certificates. 
Where prompted: "When a server requests your personal certificate" select the option "Select one automatically".
Then click on "View Certificates" and import the password protected .PFX file under "Your Certificates" tab (the password can be found in a separate file called PASSWORD.TXT).
Now you can access RavenDB by typing the URL in Firefox.

General FAQ
===========
* If the certificate is installed and you are unable to reach the RavenDB Studio and no authentication error is displayed, please go to the Cloud Portal at cloud.ravendb.net and make sure your IP address is in the allowed IPs list.

* Inside the PEM folder, we have prepared the different pieces of the certificate for you to use with your tools (OpenSSL, cURL, etc). The .CRT file contains the certificate itself, The .KEY file contains the private key and the .PEM file contains both the certificate and the private key.

* Chrome caches certificates. If you get a RavenDB authentication error, please try again in an incognito window (or close all instances of Chrome and try again).
