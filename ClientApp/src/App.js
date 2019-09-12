import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';
import LoginFrom from './components/Auth/LoginFrom';
import RegistrationForm from './components/Auth/RegistrationForm';
import ResetPassword from './components/Auth/ResetPassword/ResetPassword';

export default () => (
  <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/counter' component={Counter} />
    <Route path='/fetch-data/:startDateIndex?' component={FetchData} />
    <Route path='/login' component={LoginFrom} />
    <Route path='/registration' component={RegistrationForm} />
    <Route path='/resetPassword/:token?' component={ResetPassword} />
    {/*<PrivateRoute path='/resetPassword' component={ResetPassword}/>*/}
  </Layout>
);
