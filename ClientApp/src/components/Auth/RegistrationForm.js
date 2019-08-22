import isEmail from "validator/lib/isEmail";
import {connect} from "react-redux";
import React, {Component} from "react";
import { bindActionCreators } from 'redux';
//import {actionCreators} from "../../store/Register";
import Field from "../From/Field";


class RegistrationForm extends Component {
    
    
    state = {
        fields: this.props.fields || {
            userName: '',
            email: '',
            password: '',
        },

        fieldErrors: {},
    };
    
    onFormSubmit = (event) => {
        debugger;
        event.preventDefault();
        if (this.validate()) return false;
        this.props.register(this.state.fields)      
    };

    validate = () => {
        debugger;
        const person = this.state.fields;
        const fieldErrors = this.state.fieldErrors;
        const errMessages = Object.keys(fieldErrors).filter((keys) => fieldErrors[keys]);

        if (!person.userName) return true;
        if (!person.email) return true;
        if (!person.password) return true;       
        return !!errMessages.length;
    };


    onInputChange = ({name, value, error}) => {
        const fields = this.state.fields;
        const fieldErrors = this.state.fieldErrors;

        fields[name] = value;
        fieldErrors[name] = error;

        this.setState({fields, fieldErrors});
    };

   
    render() {
        console.log(this.serverAnswer);
        document.body.className = "session-authentication";
        return (
            <div className="">
                <div className="text-center width-full pt-5 pb-4">
                    <a className="header-logo" href="/">
                        <i className="desktop icon huge"  />
                    </a>
                </div>
                <div className="auth-form px-3">
                    <div className="auth-form-header p-0">
                        <h1>Join App</h1>
                    </div>

                    <div className="auth-form-body mt-3">
                        <div className="column">
                            <form className="ui form" onSubmit={this.onFormSubmit}>
                                <Field name="email" onChange={this.onInputChange} value={this.state.fields.email}
                                       label='Email' placeholder='Email' icon="envelope icon"
                                       validate={(val) => isEmail(val) ? false : 'Invalid Email'}/>
                                <Field name="userName" onChange={this.onInputChange} value={this.state.fields.userName}
                                       label='User Name' placeholder='User Name' icon="user icon"
                                       validate={(val) => val ? false : 'User name if Required'}/>
                                <Field name="password" onChange={this.onInputChange} value={this.state.fields.password}
                                       label='Password' placeholder='Password'  icon="lock icon" password_reset={false}
                                       validate={(val) => new RegExp('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+_=!*()@%&])(?!.*[\\s]).{6,}$').test(val) ? false : 'Invalid Password'}/>
                                <button className="btn green btn-primary btn-block" type="submit" disabled={false}>Sing in</button>
                            </form>
                        </div>
                    </div>                   
                </div>
            </div>
        );
    }
}

export default connect(
    /*state => state.registeredUser,
    dispatch => bindActionCreators(actionCreators, dispatch),*/
)(RegistrationForm);