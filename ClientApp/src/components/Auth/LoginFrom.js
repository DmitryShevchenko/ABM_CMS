import {connect} from "react-redux";
import React, {Component} from "react"
import Field from "../From/Field"
import {bindActionCreators} from 'redux';
import {actionCreators} from "../../store/Login";
import './form.css'
import GoogleLogin from 'react-google-login';
import config from '../../config';

class LoginFrom extends Component {

    componentWillReceiveProps(update) {
        this.setState({loginError: update.serverAnswer.loginError})
    }

    state = {
        fields: this.props.fields || {
            UserName: '',
            Password: '',
        },
        fieldErrors: {},
    };

    onFormSubmit = (event) => {
        event.preventDefault();
        this.props.loginAction(this.state.fields);
    };


    onInputChange = ({name, value, error}) => {
        const fields = this.state.fields;
        const fieldErrors = this.state.fieldErrors;

        fields[name] = value;
        fieldErrors[name] = error;

        this.setState({fields, fieldErrors});
    };

    googleResponse = (response) => {
        if (!response.tokenId) {
            console.error("Unable to get tokenId from Google", response);
            return;
        }

       // const token = new Blob([JSON.stringify({tokenId: response.tokenId}, null, 2)], {type: 'application/json'});
        const token = JSON.stringify(response.tokenId);
        this.props.gLoginAction(token);

    }


    render() {
        document.body.className = "session-authentication";
        return (
            <div className="">
                <div className="text-center width-full pt-5 pb-4">
                    <a className="header-logo" href="/">
                        <i className="desktop icon huge"/>
                    </a>
                </div>
                <div className="auth-form px-3">
                    <div className="auth-form-header p-0">
                        <h1>Sing up to App</h1>
                    </div>
                    {this.props.serverAnswer.statusCode.statusCode === 401 ? <div className='js-flash-container'>
                        <div className="flash flash-full flash-error">
                            <div className="container">
                                Incorrect username or password.
                            </div>
                        </div>
                    </div> : null}
                    <div className="auth-form-body mt-3">
                        <div className="column">
                            <form className="ui form" onSubmit={this.onFormSubmit}>
                                <Field name="userName" onChange={this.onInputChange} value={this.state.fields.userName}
                                       label='User Name' placeholder='User Name'
                                       icon="user icon"/>
                                <Field name="password" onChange={this.onInputChange} value={this.state.fields.password}
                                       label='Password' placeholder='Password'
                                       icon="lock icon"/>
                                <button className="btn green btn-primary btn-block" type="submit">Sing in</button>
                            </form>
                        </div>
                    </div>
                    <p className="create-account-callout mt-3">New to App? <a href="/register">Create an account.</a>
                    </p>
                    <GoogleLogin
                        clientId={config.GOOGLE_CLIENT_ID}
                        render={renderProps => (
                            <button onClick={renderProps.onClick} disabled={renderProps.disabled}>This is my custom
                                Google button</button>
                        )}
                        buttonText="Login"
                        onSuccess={this.googleResponse}
                        onFailure={this.googleResponse}
                        cookiePolicy={'single_host_origin'}
                    />
                </div>
            </div>
        );
    }
}


export default connect(
    state => state.login,
    dispatch => bindActionCreators(actionCreators, dispatch),
)(LoginFrom);
