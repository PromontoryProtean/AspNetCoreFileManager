# AspNetCoreFileManager
A web based file manager running on ASP.NET Core. The project is currently built on ASP.NET Core R2. Feedback, suggestions, contributions etc. are welcome.

This project began with a need to evaluate ImageProcessor, and grew into a proof-of-concept web application to demonstrate how various open source projects can be combined to create a web-based file manager. It can be integrated into more complex applications such as content management systems, corporate intranets etc.

The open source projects that I have integrated are:
- ImageProcessor (https://github.com/JimBobSquarePants/ImageProcessor)
- Plupload (https://github.com/moxiecode/plupload)
- Bootstrap (https://github.com/twbs/bootstrap)
- js-cookie (https://github.com/js-cookie/js-cookie)
- Font Awesome (https://github.com/FortAwesome/Font-Awesome)
- jQuery (http://jquery.com)

Some of the features are:
- Detail and tile views.
- Image thumbnails in tile view.
- Multi-file upload with progress monitoring.
- Public and secure directories.
- Bulk delete.

TODO:
- Image thumbnails in tile view for secure directory.
- Click a file in tile view to see file details such as name, path, date/time details, size, image dimensions and image preview.
- Configuration system to set properties such as the upload directories.
- Security system to make sure that users cannot upload and execute malicious files.
- Use ASP.NET tooling for downloading and building javascript libraries.
- Replace TempData messaging system with UiNotificationBus.