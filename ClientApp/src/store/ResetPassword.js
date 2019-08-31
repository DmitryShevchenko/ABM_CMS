const RESET_PASSWORD_REQUEST = "RESET_PASSWORD_REQUEST";

function resetPasswordRequest() {
    return {type: RESET_PASSWORD_REQUEST}
}

const RESET_PASSWORD_REQUEST_SUCCEED = "RESET_PASSWORD_REQUEST_SUCCEED";

function resetPasswordRequestSucceed(data) {
    return {type: RESET_PASSWORD_REQUEST_SUCCEED, payload: data}
}

const RESET_PASSWORD_REQUEST_ERROR = "RESET_PASSWORD_REQUEST_ERROR";

function resetPasswordRequestError(err) {
    return {type: RESET_PASSWORD_REQUEST_ERROR, payload: err}
}

const RESET_PASSWORD_CONFIRM_REQUEST = "RESET_PASSWORD_CONFIRM_REQUEST";

function resetPasswordConfirmRequest() {
    return {type: RESET_PASSWORD_CONFIRM_REQUEST}
}

const RESET_PASSWORD_CONFIRM_REQUEST_SUCCEED = "RESET_PASSWORD_CONFIRM_REQUEST_SUCCEED";

function resetPasswordConfirmRequestSucceed(data) {
    return {type: RESET_PASSWORD_CONFIRM_REQUEST_SUCCEED, payload: data}
}

const RESET_PASSWORD_CONFIRM_REQUEST_ERROR = "RESET_PASSWORD_CONFIRM_REQUEST_ERROR";

function resetPasswordConfirmRequestError(err) {
    return {type: RESET_PASSWORD_CONFIRM_REQUEST_ERROR, payload: err}
}

export const actionCreators = {
    resetPassword: (data) => (dispatch) => {
        dispatch(resetPasswordRequest());
        apiClient.resetPasswordFetchRequest(data)
            .then(res => dispatch(resetPasswordRequestSucceed(res)))
            .catch(err => dispatch(resetPasswordRequestError(err)));
    },
    resetPasswordConfirm: (data) => (dispatch) => {
        dispatch(resetPasswordConfirmRequest());
        apiClient.resetPasswordConfirmFetchRequest(data)
            .then(res => dispatch(resetPasswordConfirmRequestSucceed(res)))
            .catch(err => dispatch(resetPasswordConfirmRequestError(err)));

    }
};

const apiClient = {
    resetPasswordFetchRequest: async (data) => {
        debugger;
        return await fetch('api/account/ResetPassword', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        }).then(res => res.json())
            .catch(err => console.log('Error', err));
    },

    resetPasswordConfirmFetchRequest: async (data) => {
        debugger;
        return await fetch('api/account/ResetPasswordConfirm', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        }).then(res => res.json())
            .catch(err => console.log('Error', err));
    }

};

const initialState = {
    Response: {}
};

export const reducer = (state, action) => {
    state = state || initialState;
    switch (action.type) {
        case RESET_PASSWORD_REQUEST:
            return {...state};
        case RESET_PASSWORD_REQUEST_SUCCEED:
            return {
                ...state, Response: action.payload
            };
        case RESET_PASSWORD_CONFIRM_REQUEST:
            return {...state};
        case RESET_PASSWORD_CONFIRM_REQUEST_SUCCEED:
            return {
                ...state, Response: action.payload
            };
        default:
            return state;

    }
};


