import {connect} from "react-redux";
import React, {Component} from "react"
import {bindActionCreators} from 'redux';
import {actionCreators} from "../../../store/ResetPassword";


class ConfirmPassword extends Component {

    //ref={(input) => {this.textInput = input}

    formOnSubmit = () =>{};

    

    render() {       
       
        
        return(
            <div>
                
                <form onSubmit={this.formOnSubmit}>
                    <input type="password" name='password' placeholder="Password" ref={(input) => {this.password = input}}/>
                    <input type="password" name='confirmPassword' placeholder="Confirm Password" ref={(input) => {this.confirmPassword = input}}/>
                    <button type="submit">Submit</button>
                </form>
            </div>
        );
    }
}

export default connect(
)(ConfirmPassword);
