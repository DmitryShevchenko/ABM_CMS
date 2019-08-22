import PropTypes from "prop-types";
import React, {Component} from "react"
import './Field.css';



export default class Field extends Component {
    
    static propTypes = {
        label: PropTypes.string,
        placeholder: PropTypes.string,
        name: PropTypes.string.isRequired,
        value: PropTypes.string,
        validate: PropTypes.func,
        onChange: PropTypes.func.isRequired,
        icon: PropTypes.string,
        password_reset: PropTypes.bool,
    };

    UNSAFE_componentWillReceiveProps(update) {
        this.setState({value: update.value});
    }

    state = {
        value: this.props.value,
        error: false,
    };
   

    onInputChange = (evt) => {
        const name = this.props.name;
        const value = evt.target.value;
        const error = this.props.validate ? this.props.validate(value) : false;

        this.setState({value, error});

        this.props.onChange({name, value, error});
    };

    render() {
        return (
            <div>
                <div className="field">
                    <div className='space-between'>
                        <label>{this.props.label}</label>
                        {(this.props.name === "password" || this.props.name === 'Password') && this.props.password_reset ?
                            <a href="/password_reset">Forgot password?</a> : null}
                    </div>
                    <div className={this.props.icon ? "ui left icon input" : ""}>
                        <input placeholder={this.props.placeholder} value={this.props.value}
                               onChange={this.onInputChange}/>
                        <i className={this.props.icon}/>
                    </div>

                </div>
                <span style={{color: 'red'}}>{this.state.error}</span>
            </div>
        )
    }
}

