
let ajax = function () {
    return {
        post: function (data, path, token) {
            var promise = new Promise(function (resolve, reject) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", path);
                xhr.setRequestHeader('Content-Type', 'application/json');
                xhr.setRequestHeader("SToken", token);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        resolve(response);
                    } else {
                        reject();
                    }
                };
                xhr.send(JSON.stringify(data));
            });

            return promise;
        },
        get: function (path, token) {
            var promise = new Promise(function (resolve, reject) {
                var xhr = new XMLHttpRequest();
                xhr.open("GET", path);
                xhr.setRequestHeader('Content-Type', 'application/json');
                xhr.setRequestHeader("SToken", token);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        resolve(response);
                    } else {
                        reject();
                    }
                };
                xhr.send();
            });

            return promise;
        },
        upload: function (formData, metastring, token) {
            var promise = new Promise(function (resolve, reject) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", "/api/file/UploadFile/" + metastring);
                xhr.setRequestHeader("SToken", token);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        resolve(response);
                    } else {
                        reject();
                    }
                };
                xhr.send(formData);
            });

            return promise;
        }
    }
};


export { ajax as ajax};