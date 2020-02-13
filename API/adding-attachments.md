## How to upload and attach files from third party applications

The basic flow of uploading files in Lime Field looks like this:

1. Create a `File` entity
2. Upload the file's content
3. Add the file entity as attachment to another entity, e.g. `Case`, `Unit`, `Contract`

__Note__, you probably will need to ensure that CORS requests are allowed from the domain where your application is hosted. Contact __Lime Field__ about this.

So, lets see how the uploads can be done with a code example.

### Setup and Helpers

First we will define some basic variables and helper functions to make the example code more readable. Hopefully.

You are going to need an API access token, please follow the steps described in [How to generate an access token](generate-api-tokens.md).

The `entityHref` should point out the entity to which we add attachments, e.g. `cases/1`, `units/1`, etc.

```javascript
const username = 'YOUR_LIME_FIELD_USERNAME';
const apiToken = 'YOUR_LIME_FIELD_API_TOKEN';
const credentials = window.btoa(`${username}:${apiToken}`);
const baseurl = 'https://YOUR_LIME_FIELD_INSTALLATION_NAME.remotex.net/api';
const entityHref = 'entity/id';

function requestHeaders(options = {}) {
  const result = new Headers({
    'Accept': 'application/json',
    'Authorization': `Basic ${credentials}`,
    'Content-Type': 'application/json'
  });

  for (let header in options) {
    result.set(header, options[header]);
  }

  return result;
}

function requestOptions(options = {}, headers = {}) {
  return Object.assign({
    method: 'POST',
    cache: 'no-cache',
    credentials: 'include',
    headers: requestHeaders(headers)
  }, options);
}
```

### 1. Creating a `File` entity

```javascript
async function createFileEntity(file) {
  const entity = {
    Title: file.name,
    FileName: file.name,
    ContentType: file.type,
    Length: file.size
  };

  return fetch(`${baseurl}/files`, requestOptions({
    body: JSON.stringify(entity)
  }))
  .then(async response => ({
    entity: JSON.parse(await response.text()),
    file: file
  }));
}
```

### 2. Uploading the file content

```javascript
async function uploadFileContent(data) {
  return fetch(`${baseurl}/${data.entity.Content.Href}`, requestOptions({
    method: 'PUT',
    body: data.file
  }, {
    'Accept': '*/*',
    'Content-Type': data.file.type
  }));
}
```

### 3. Adding the files as attachments

__Note__, in this example we allow for multiple files uploads. But it works for a single file as well.

```javascript
async function addAttachmentsToEntity(attachments) {
  const commands = attachments.map(attachment => ({
    Name: 'AddAttachmentToEntity',
    Target: entityHref,
    Parameters: [{
      Name: 'File', Values: [attachment.entity.Href]
    }]
  }));

  return fetch(`${baseurl}/commands`, requestOptions({
    body: JSON.stringify({Commands: commands})
  }));
}
```

### Bootstraping the code to a view

Bind `openFilesManager` to an `onclick` event on some button in your web page.

__Note__, we do not await content upload requests. They can be executed in parallel.

```javascript
function isFileToLarge(file) {
  return file && file.size > 10485760;
}

function openFilesManager() {
  const input = document.createElement('input');
  input.setAttribute('type', 'file');
  input.setAttribute('accept', '*');
  input.setAttribute('multiple', true);

  input.addEventListener('change', event => {
    const files = [...event.target.files];

    if (files.some(isFileToLarge)) {
      throw 'One of the files is to large';
    }

    Promise.all(files.map(createFileEntity)).then(attachments => {
      Promise.all(attachments.map(uploadFileContent)).catch(console.error);

      return Promise.resolve(addAttachmentsToEntity(attachments));
    })
    .then(response => response.text())
    .then(response => console.log(JSON.parse(response)))
    .catch(console.error);
  });

  input.click();
}
```

### Finally. The complete code example.

<details>
<summary>View the code</summary>

```javascript
const username = 'YOUR_LIME_FIELD_USERNAME';
const apiToken = 'YOUR_LIME_FIELD_API_TOKEN';
const credentials = window.btoa(`${username}:${apiToken}`);
const baseurl = 'https://YOUR_LIME_FIELD_INSTALLATION_NAME.remotex.net/api';
const entityHref = 'entity/id';

function requestHeaders(options = {}) {
  const result = new Headers({
    'Accept': 'application/json',
    'Authorization': `Basic ${credentials}`,
    'Content-Type': 'application/json'
  });

  for (let header in options) {
    result.set(header, options[header]);
  }

  return result;
}

function requestOptions(options = {}, headers = {}) {
  return Object.assign({
    method: 'POST',
    cache: 'no-cache',
    credentials: 'include',
    headers: requestHeaders(headers)
  }, options);
}

async function createFileEntity(file) {
  const entity = {
    Title: file.name,
    FileName: file.name,
    ContentType: file.type,
    Length: file.size
  };

  return fetch(`${baseurl}/files`, requestOptions({
    body: JSON.stringify(entity)
  }))
  .then(async response => ({
    entity: JSON.parse(await response.text()),
    file: file
  }));
}

async function uploadFileContent(data) {
  return fetch(`${baseurl}/${data.entity.Content.Href}`, requestOptions({
    method: 'PUT',
    body: data.file
  }, {
    'Accept': '*/*',
    'Content-Type': data.file.type
  }));
}

async function addAttachmentsToEntity(attachments) {
  const commands = attachments.map(attachment => ({
    Name: 'AddAttachmentToEntity',
    Target: entityHref,
    Parameters: [{
      Name: 'File', Values: [attachment.entity.Href]
    }]
  }));

  return fetch(`${baseurl}/commands`, requestOptions({
    body: JSON.stringify({Commands: commands})
  }));
}

function isFileToLarge(file) {
  return file && file.size > 10485760;
}

function openFilesManager() {
  const input = document.createElement('input');
  input.setAttribute('type', 'file');
  input.setAttribute('accept', '*');
  input.setAttribute('multiple', true);

  input.addEventListener('change', event => {
    const files = [...event.target.files];

    if (files.some(isFileToLarge)) {
      throw 'One of the files is to large';
    }

    Promise.all(files.map(createFileEntity)).then(attachments => {
      Promise.all(attachments.map(uploadFileContent)).catch(console.error);

      return Promise.resolve(addAttachmentsToEntity(attachments));
    })
    .then(response => response.text())
    .then(response => console.log(JSON.parse(response)))
    .catch(console.error);
  });

  input.click();
}
```
</details>
