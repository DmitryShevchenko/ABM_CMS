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

export const actionCreators = {
    login: (data) => (dispatch) => {
        dispatch(loginRequest());
        apiClient.loginFetchRequest(data)
            .then(res => dispatch(loginRequestSucceed(res)))
            .catch(err => dispatch(loginRequestError(err)));
    }
};

const apiClient = {
    loginFetchRequest: async (data) => {
        return await fetch('api/account/login', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        }).then(res => res.json())
            .catch(err => console.log('Error', err))
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
    isLoading: false,    
};

export const reducer = (state, action) => {
    state = state || initialState;
    switch (action.type) {
        case LOGIN_REQUEST:
            return{
                ...state,
                isLoading: true
            };
        case LOGIN_REQUEST_SUCCEED:
            debugger;
            return {
                ...state, isLoading: false, serverAnswer: action.payload
            };
        case LOGIN_REQUEST_ERROR:
            return {
                ...state, isLoading: false, serverAnswer: action.payload
            };
        default:
            return state;
            
    }
};


