import React, {Component} from "react"
import {connect} from "react-redux";



class AuthForm extends Component {
    loggedIn = false;

    render() {
        if (this.props.loggedIn) {
            return (
                <button className="ui button">Click Here</button>
            );
        } else {
            return (
                <div>
                    <div className="ui large buttons">
                        <a className="ui button" href='/login'>Login</a>
                        <div className="or"/>
                        <a className="ui button" href='/register'>Register</a>
                    </div>                   
                </div>

            );
        }
    }
}

export default connect(
    state => ({}),
    dispatch => ({}),
)(AuthForm);



