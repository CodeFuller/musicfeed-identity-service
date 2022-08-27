import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom'
import { Provider } from 'react-redux';
import AuthProvider from './utils/authProvider'
import userManager from './services/userService'
import store from './store';
import Home from './pages/home';
import SigninOidc from './pages/signin-oidc';
import SignoutOidc from './pages/signout-oidc';
import './custom.css';

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Provider store={store}>
        <AuthProvider userManager={userManager} store={store}>
          <Router>
              <Switch>
                <Route path="/signin-oidc" component={SigninOidc} />
                <Route path="/signout-oidc" component={SignoutOidc} />
                <Route path="/" component={Home} />
              </Switch>
            </Router>
        </AuthProvider>
      </Provider>
    );
  }
}
