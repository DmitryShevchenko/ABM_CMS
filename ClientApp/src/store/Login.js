import config from '../config';

const LOGIN_REQUEST = "LOGIN_REQUEST";

function loginRequest() {
    return {type: LOGIN_REQUEST}
}

const LOGIN_REQUEST_SUCCEED = "LOGIN_REQUEST_SUCCEED";

function loginRequestSucceed(data) {
    return {type: LOGIN_REQUEST_SUCCEED, payload: data}
}

const LOGIN_REQUEST_ERROR = "LOGIN_REQUEST_ERROR";

function loginRequestError(err) {
    return {type: LOGIN_REQUEST_SUCCEED, payload: err}
}

const G_LOGIN_REQUEST = "G_LOGIN_REQUEST";

function gLoginRequest() {
    return {type: G_LOGIN_REQUEST}
}

const G_LOGIN_REQUEST_SUCCEED = "G_LOGIN_REQUEST_SUCCEED";

function gLoginRequestSucceed(data) {
    return {type: G_LOGIN_REQUEST_SUCCEED, payload: data}
}

const G_LOGIN_REQUEST_ERROR = "G_LOGIN_REQUEST_ERROR";

function gLoginRequestError(err) {
    return {type: G_LOGIN_REQUEST_ERROR, payload: err}
}

export const actionCreators = {
    loginAction: (data) => (dispatch) => {
        dispatch(loginRequest());
        apiClient.login(data)
            .then(res => dispatch(loginRequestSucceed(res)))
            .catch(err => dispatch(loginRequestError(err)));
    },
    gLoginAction: (data) => (dispatch) => {
        dispatch(gLoginRequest());
        apiClient.gLogin(data).then(res => dispatch(gLoginRequestSucceed(res.token)))
    }
};

const apiClient = {
    login: async (data) => {
        return await fetch('api/account/login', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        }).then(res => res.json())
            .catch(err => console.log('Error', err))
    },
    gLogin: async (data) => {
        debugger;
        return await fetch(config.GOOGLE_AUTH_CALLBACK_URL, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            mode: "cors",
            cache: "default",
            body: data
        }).then(res => res.json());
    }
};

const initialState = {
    serverAnswer: {
        token: null,
        expiration: null,
        userName: null,
        userRole: null,
        loginError: null,
        statusCode: {
            statusCode: null,
        },
    },
    user: '',
    isAuthenticated: false,
    isLoading: false,
};

export const reducer = (state, action) => {
    state = state || initialState;
    switch (action.type) {
        case LOGIN_REQUEST:
            return {
                ...state,
                isLoading: true
            };
        case LOGIN_REQUEST_SUCCEED:
            return {
                ...state, isLoading: false, serverAnswer: action.payload
            };
        case G_LOGIN_REQUEST_SUCCEED:
            return {
                ...state, user: action.payload, isAuthenticated: true
            };       
        default:
            return state;

    }
};


