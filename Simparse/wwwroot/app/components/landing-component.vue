<template>
    <div>
        <div class="fixed-top">
            <nav class="navbar navbar-light bg-light">
                <a style="font-size:30px;cursor:pointer" @click="showLandingFn" class="navbar-brand"> &nbsp;&nbsp;&nbsp;Simparse</a>
                <form class="form-inline">
                    <button class="btn btn-lg btn-outline-primary" @click="showLoginFn" type="button"><i class="fas fa-sign-in-alt"></i> Login</button>
                    &nbsp;&nbsp;&nbsp;
                    <button class="btn btn-lg btn-outline-success" style="margin-right:50px;" @click="showRegisterFn" type="button"><i class="fas fa-door-open"></i>  Register</button>
                </form>
            </nav>
        </div>


        <div v-if="showLanding">

            <div class="row" style="padding-top: 50px;">
                <div class="col-12">
                    <div class="card text-center">
                        <div class="card-body" style="background-color:#dfe7fb;">
                            <h3 class="card-title">Text Extraction Simplified</h3>
                            <p class="card-text">Transform your PDF and image files into more workable spreadsheets and JSON files.</p>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row" style="padding-top:80px;padding-bottom:100px;">
                <div class="col d-flex justify-content-center">
                    <div class="card text-center" style="width: 18rem;">
                        <div class="card-body">
                            <h5 class="card-title">Easy to Use</h5>
                            <p class="card-text">Upload your PDF or image files into our easy to use process. Organize your files into folders using our intuitive UI and export your data at the folder level or the indivual file level.</p>
                            <a href="#workflow" class="btn btn-primary">Workflow</a>
                        </div>
                    </div>
                </div>
                <div class="col d-flex justify-content-center">
                    <div class="card text-center" style="width: 18rem;">
                        <div class="card-body">
                            <h5 class="card-title">Advanced OCR</h5>
                            <p class="card-text">We pride ourselves in our technological choices offering advanced OCR and maching learning processes. </p>
                            <br />
                            <br />
                            <a href="#technology" class="btn btn-primary">Technology</a>
                        </div>
                    </div>
                </div>
                <div class="col d-flex justify-content-center">
                    <div class="card text-center" style="width: 18rem;">
                        <div class="card-body">
                            <h5 class="card-title">Customization</h5>
                            <p class="card-text">If you dont find a featureset the meets your needs our top notch development team can customize any solution.</p>
                            <br />
                            <br />
                            <a href="#customize" class="btn btn-primary">Customization</a>
                        </div>
                    </div>
                </div>
                <div class="col d-flex justify-content-center">
                    <div class="card text-center" style="width: 18rem;">
                        <div class="card-body">
                            <h5 class="card-title">Security</h5>
                            <p class="card-text">Located and built inside the US, Simparse is a company that takes security and privacy seriously. All data is encrypted and stored in the Google Cloud.</p>
                            <br />
                            <a href="#security" class="btn btn-primary">Security</a>
                        </div>
                    </div>
                </div>
            </div>


            <div style="background-color: #dfe7fb;padding-top: 80px;padding-bottom: 100px;">
                <div class="container">
                    <div class="row" style="margin-top:100px;">
                        <div class="col">
                            <b>Due to this being such a new company everything is currently free to use. We will implement billing at a later time. Feel welcome to test our product and leave feedback.</b>
                            <b>Pay As You Go</b>
                            <p>Start your free demo today with no payment information necessary. We bill per page on each document with zero subscription fees.</p>
                            <ul>
                                <li> 25 cents Per Page with simple monthly billing model</li>
                                <li> First 20 pages are free </li>
                                <li> No subscription required </li>
                                <li> Google Pay is accepted </li>
                            </ul>
                            <br />
                        </div>
                    </div>
                </div>
            </div>

            <div style="padding-top: 80px;padding-bottom: 100px;" id="workflow">
                <div class="container">
                    <div class="row" style="margin-top:100px;">
                        <div class="col">
                            <b>Easy To Use Workflow</b>
                            <ul>
                                <li> Step 1) Create a folder for your documents. Each folder will apply a given set of mappings to all files in that folder</li>
                                <li> Step 2) Upload your documents to a given folder.</li>
                                <li> Step 3) Use the highlighter tool to map points of interest in your documents for that folder. </li>
                                <li> Step 4) Download all the items in your folder to a spreadsheet or a JSON file on a per file or per folder basis. </li>
                            </ul>
                            <br />
                        </div>
                    </div>
                </div>
            </div>


            <div style="background-color: #dfe7fb;padding-top: 80px;padding-bottom: 100px;" id="technology">
                <div class="container">
                    <div class="row" style="margin-top:100px;">
                        <div class="col">
                            <b>Cutting Edge Technology</b>
                            <p>We use the latest technology provided by the Google Cloud integrated into our proprietary software. Using existing and custom machine learning algorithms to provide industry standard OCR text extraction and accuracy. </p>
                            <br />
                        </div>
                    </div>
                </div>
            </div>


            <div style="padding-top: 80px;padding-bottom: 100px;" id="customize">
                <div class="container">
                    <div class="row" style="margin-top:100px;">
                        <div class="col">
                            <b>We Build To Suit Your Needs</b>
                            <p>Unfortuantly not all documents are created equally. Our standard process will give most users the text extraction capabilties they are looking for. While some users will have more complex use cases. This is where our highly qualified team of developers and data scientisis shine. We can build custom ML models to meet your needs.  </p>
                            <br />
                        </div>
                    </div>
                </div>
            </div>

            <div style="background-color: #dfe7fb;padding-top: 80px;padding-bottom: 100px;" id="security">
                <div class="container">
                    <div class="row" style="margin-top:100px;">
                        <div class="col">
                            <b>Security</b>
                            <ul>
                                <li> End to end TLS 1.2 encyption</li>
                                <li> All user data is encrypted and stored in Google Identity platform </li>
                                <li> All documents are encrypyted and protected by the Google Cloud</li>
                                <li> US based development team </li>
                                <li> 100% Privacy guarantee </li>
                            </ul>
                            <br />
                        </div>
                    </div>
                </div>
            </div>


            <div style="padding-top: 80px;padding-bottom: 100px;">
                <div class="container">
                    <div class="row" style="margin-top:100px;">
                        <div class="col">
                            <b>About Us</b>
                            <p>We are young start-up based out of Netpune Beach, Florida. Comprised of a team of experienced data scientists and software engineers with decades of experience providing custom software solutions and cloud implementations.</p>
                            <br />
                        </div>
                    </div>
                </div>
            </div>
        </div>





        <div class="row" v-if="showLogin">
            <div class="col d-flex justify-content-center" style="margin-top:300px;">
                <div class="card text-center" style="width: 18rem;">
                    <div class="card-body">
                        <h5 class="card-title">Login</h5>
                        <div style="padding-top:40px;">
                            <div class="input-group mb-3">
                                <input type="email" style="max-width:300px;" class="form-control" placeholder="Email" aria-label="Email" v-model="loginEmail">
                            </div>
                            <div class="input-group mb-3">
                                <input type="password" style="max-width:300px;" class="form-control" placeholder="Password" aria-label="Password" v-model="loginPassword">
                            </div>
                            <div class="input-group mb-3 justify-content-center">
                                <button type="button" class="btn btn-success" @click="loginClick">Login</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>





        <div class="row" v-if="showRegistration">
            <div class="col d-flex justify-content-center" style="margin-top:300px;">
                <div class="card text-center" style="width: 18rem;">
                    <div class="card-body">
                        <h5 class="card-title">Register</h5>
                        <div style="padding-top:40px;">
                            <div class="input-group mb-3">
                                <input type="text" style="max-width:300px;" class="form-control" placeholder="First Name" aria-label="First Name" v-model="registerFirstName">
                            </div>
                            <div class="input-group mb-3">
                                <input type="text" style="max-width:300px;" class="form-control" placeholder="Last Name" aria-label="Last Name" v-model="registerLastName">
                            </div>
                            <div class="input-group mb-3">
                                <input type="email" style="max-width:300px;" class="form-control" placeholder="Register Email" aria-label="Register Email" v-model="registerEmail">
                            </div>
                            <div class="input-group mb-3">
                                <input type="password" style="max-width:300px;" class="form-control" placeholder="Register Password" aria-label="Register Password" v-model="registerPassword">
                            </div>
                            <div class="input-group mb-3">
                                <input type="password" style="max-width:300px;" class="form-control" placeholder="Confirm Password" aria-label="Confirm Password" v-model="confirmRegisterPassword">
                            </div>
                            <div class="input-group mb-3 d-flex justify-content-center">
                                <button type="button" class="btn btn-success" @click="registerClick">Register</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>



        <footer-comp></footer-comp>
    </div>


</template>


<script>
    $.postJSON = function (url, data, callback) {
        return jQuery.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            'type': 'POST',
            'url': url,
            'data': JSON.stringify(data),
            'dataType': 'json',
            'success': callback
        });
    };

    import footer from './footer-component.vue';

    let landingComponent = {
        data: function () {
            return {
                loginEmail: "",
                loginPassword: "",
                registerFirstName: "",
                registerLastName: "",
                registerEmail: "",
                registerPassword: "",
                confirmRegisterPassword: "",
                showLanding: true,
                showRegistration: false,
                showLogin: false
            }
        },
        components: {
            "footer-comp": footer
        },
        methods: {
            showRegisterFn() {
                this.showLanding = false;
                this.showLogin = false;
                this.showRegistration = true;
            },
            showLoginFn() {
                this.showLanding = false;
                this.showLogin = true;
                this.showRegistration = false;
            },
            showLandingFn() {
                this.showLanding = true;
                this.showLogin = false;
                this.showRegistration = false;
            },
            loginClick: function () {

                var request = {
                    Email: this.loginEmail,
                    Password: this.loginPassword
                };

                $.postJSON("api/Identity/login", request, function (response) {

                    Cookies.set('authInfo', response.token);
                    window.location.href = '/dashboard';

                }).fail(function (e) {
                    console.log("fail");
                });

            },
            registerClick: function () {

            }
        },
        mounted: function () {
            var token = Cookies.get('authInfo');
            if (token) {
                window.location.href = '/dashboard';
            }

        }
    };

    export { landingComponent as default }
</script>

