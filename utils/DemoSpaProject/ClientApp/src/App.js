import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom'
import { Provider } from 'react-redux';
import AuthProvider from './utils/authProvider'
import userManager from './services/userService'
import store from './store';
import { Layout } from './components/Layout'
import { PrivateRoute } from './utils/PrivateRoute'
import { LoginPage } from './pages/LoginPage';
import { HomePage } from './pages/HomePage';
import { IdentityPage } from './pages/IdentityPage';
import { SigninOidc } from './pages/signin-oidc';
import { SignoutOidc } from './pages/signout-oidc';
import './custom.css';

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Provider store={store}>
        <AuthProvider userManager={userManager} store={store}>
            <Router>
              <Layout>
              <Switch>
                <Route path="/login" component={LoginPage}  />
                <Route path="/signin-oidc" component={SigninOidc} />
                <Route path="/signout-oidc" component={SignoutOidc} />
                <PrivateRoute path="/identity" component={IdentityPage} />
                <PrivateRoute exact path="/" component={HomePage} />
              </Switch>
              </Layout>
            </Router>
        </AuthProvider>
      </Provider>
    );
  }
}
