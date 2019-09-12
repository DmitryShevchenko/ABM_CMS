import {connect} from "react-redux";
import React, {Component} from "react"
import {bindActionCreators} from 'redux';
import {actionCreators} from "../../../store/ResetPassword";


class ResetPassword extends Component {

    //ref={(input) => {this.textInput = input}
    /*ref={(input) => {this.email = input}}*/
    

    token = decodeURIComponent(this.props.match.params.token);

    state = {
        response: null,
        Message: '',
        ErrorMessage: '',
    };

    componentWillReceiveProps(update) {
        if (update.Response.OkResult){
            this.setState({Message: update.Response.OkResult})
        }
    }

    componentDidMount() {
        debugger;
        if (this.token !== undefined) {
            fetch("api/Token/VerifyUserToken", {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify(this.token),
            }).then(res => this.setState({response: res.json()}))
                .catch(err => console.log(err));
        }
    }

    formEmailOnSubmit = (event) => {
        event.preventDefault();
        this.props.resetPassword(this.email.value);
        this.email.value = '';
    };
    formPasswordOnSubmit = (event) => {
        event.preventDefault();
        debugger;
        if (this.verifyConfirmPassword(this.password.value, this.confirmPassword.value)) {
            this.props.resetPasswordConfirm({token: this.token, password: this.password.value})
        } else {
            this.setState({Message: "Passwords do not match."})
        }

    };

    verifyConfirmPassword = (password, confirmPassword) => password === confirmPassword;


    render() {
        return (
            <div>
                {this.token === undefined ?
                    
                    <form onSubmit={this.formEmailOnSubmit}>
                        <input type="email" name='email' placeholder="Email" ref={(input) => {
                            this.email = input}}/>
                        <button type="submit">Submit</button>
                    </form> :
                    <div>
                        {/*<Route exact path='/resetPassword/:userId/:token' component={ConfirmPassword} />*/}
                        {this.props.Response.error ?
                            <div className="flash flash-full flash-error">
                                <div className="container">
                                    {this.state.response.error}
                                </div>
                            </div> : <div>
                                {this.state.Message !== '' ?
                                    <div className="flash flash-full flash-error">
                                        <div className="container">
                                            {this.state.Message}
                                        </div>
                                    </div> : null}
                                <form onSubmit={this.formPasswordOnSubmit}>
                                    <input type="password" name='password' placeholder="Password" ref={(input) => {
                                        this.password = input}}/>
                                    <input type="password" name='confirmPassword' placeholder="Confirm Password"
                                           ref={(input) => {
                                               this.confirmPassword = input}}/>
                                    <button type="submit">Submit</button>
                                </form>
                            </div>}
                    </div>
                }
            </div>
        );
    }
}

export default connect(
    state => state.resetPassword,
    dispatch => bindActionCreators(actionCreators, dispatch),
)(ResetPassword);
