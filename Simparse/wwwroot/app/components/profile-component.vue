<template>
    <div class="row">
        <div class="col">
            <h3>Profile</h3>
            <div class="row">
                <div class="col-4">
                    <div class="form-group">
                        <label>First Name</label>
                        <input type="text" v-model="currentUser.firstName" class="form-control" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="form-group">
                        <label>Last Name</label>
                        <input type="text" v-model="currentUser.firstName" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <div class="form-group">
                        <label>Address Line 1</label>
                        <input type="text" v-model="currentUser.addressLineOne" class="form-control" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="form-group">
                        <label>Address Line 2</label>
                        <input type="text" v-model="currentUser.addressLineTwo" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <div class="form-group">
                        <label>City</label>
                        <input type="text" v-model="currentUser.city" class="form-control" />
                    </div>
                </div>
                <div class="col">
                    <div class="form-group">
                        <label>State</label>
                        <input type="text" v-model="currentUser.state" class="form-control" />
                    </div>
                </div>
                <div class="col">
                    <div class="form-group">
                        <label>Zip</label>
                        <input type="text" v-model="currentUser.zip" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <button class="btn btn-primary" @click="saveUserData">Update</button>
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col">
                    <h2>Change Password</h2>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <div class="form-group">
                        <label>Current Password</label>
                        <input type="text" v-model="changePassCurrent" class="form-control" />
                    </div>
                </div>
                <div class="col">
                    <div class="form-group">
                        <label>New Password</label>
                        <input type="text" v-model="changePassNew" class="form-control" />
                    </div>
                </div>
                <div class="col">
                    <div class="form-group">
                        <label>Confirm Password</label>
                        <input type="text" v-model="changePassConfirm" class="form-control" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <button class="btn btn-success" @click="updatePassword">Change Password</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>

    import { ajax } from '../ajax.js';
    let ajaxhelper = new ajax();

    export default {
        props: ['currentUser'],
        data() {
            return {
                changePassCurrent: "",
                changePassNew: "",
                changePassConfirm: "",
            };
        },
        methods: {
            saveUserData() {
                ajaxhelper.post(this.currentUser, "/api/identity/postUserData", this.token).then(function () {

                }).catch(function (e) {

                });
            },
            updatePassword() {
                var request = {
                    changePassCurrent: this.changePassCurrent,
                    changePassNew: this.changePassNew,
                    changePassConfirm: this.changePassConfirm
                };

                ajaxhelper.post(request, "/api/identity/updatePassword", this.token).then(function () {

                }).catch(function (e) {

                });
            },
        }
    }
</script>