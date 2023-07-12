/*import {AuthLoginCommand, LocalJwt} from "../types/AuthTypes.ts";
import React, {useContext, useState} from "react";
import {Button, Form, Grid, Header, Icon, Segment} from "semantic-ui-react";
import api from "../utils/axiosInstance.ts";
import {getClaimsFromJwt} from "../utils/jwtHelper.ts";
import {toast} from "react-toastify";
import { useNavigate } from "react-router-dom";
import {AppUserContext} from "../context/StateContext.tsx";


const BASE_URL = import.meta.env.VITE_API_URL;

/*export type LoginPageProps = {

}*/
/*
function LoginPage() {

    const { setAppUser } = useContext(AppUserContext);

    const navigate = useNavigate();

    const [authLoginCommand, setAuthLoginCommand] = useState<AuthLoginCommand>({email:"",password:""});

    const handleSubmit = async (event:React.FormEvent) => {

        event.preventDefault();

        try {
            const response = await api.post("/Authentication/Login", authLoginCommand);

            if(response.status === 200){
                const accessToken = response.data.accessToken;
                const { uid, email, given_name, family_name} = getClaimsFromJwt(accessToken);
                const expires:string = response.data.expires;

                setAppUser({ id:uid, email, firstName:given_name, lastName:family_name, expires, accessToken });

                const localJwt:LocalJwt ={
                    accessToken,
                    expires
                }

                localStorage.setItem("upstorage_user",JSON.stringify(localJwt));
                navigate("/");
            } else{
                toast.error(response.statusText);
            }
        } catch (error) {
            toast.error("Something went wrong!");
        }
    }

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setAuthLoginCommand({
            ...authLoginCommand,
            [event.target.name]: event.target.value
        });
    }

    const onGoogleLoginClick = (e:React.FormEvent) => {
        e.preventDefault();

        window.location.href = `${BASE_URL}/Authentication/GoogleSignInStart`;
    };

    return (
        <Grid textAlign='center' style={{ height: '100vh' }}>
            <Grid.Column style={{ maxWidth: 450 }}>
                <Header as='h2' color='teal' textAlign='center'>
                    Log-in to your account
                </Header>
                <Form size='large' onSubmit={handleSubmit}>
                    <Segment stacked>
                        <Form.Input
                            fluid
                            icon='mail'
                            iconPosition='left'
                            placeholder='Email'
                            value={authLoginCommand.email}
                            onChange={handleInputChange}
                            name="email"
                        />
                        <Form.Input
                            fluid
                            icon='lock'
                            iconPosition='left'
                            placeholder='Password'
                            type='password'
                            value={authLoginCommand.password}
                            onChange={handleInputChange}
                            name="password"
                        />

                        <Button color='teal' fluid size='large' type="submit">
                            Login
                        </Button>

                        <Button color='red' fluid onClick={onGoogleLoginClick} size='large' style={{marginTop:"5px"}} type="button">
                            <Icon name='google' /> Sign in with Google
                        </Button>
                    </Segment>
                </Form>
            </Grid.Column>
        </Grid>
    );
}

export default LoginPage;
*/


import React, { useState } from "react";
import {Grid, Image} from "semantic-ui-react";
import './LoginPage.css';
import {Link} from "react-router-dom";

const Login = () => {
    const [isFormOpen, setFormOpen] = useState(false);
    const [isSignUpActive, setSignUpActive] = useState(false);
    const [passwordVisibility, setPasswordVisibility] = useState({
        login: false,
        signup: false,
        confirm: false
    });

    const toggleForm = () => setFormOpen(!isFormOpen);
    const switchForm = (event: React.MouseEvent) => {
        event.preventDefault();
        setSignUpActive(!isSignUpActive);
    };
   /* <Image src='./bg.jpg' size='medium' centered style={{ marginTop: '1em' }} /> */
    const togglePasswordVisibility = (field: string) => {
        setPasswordVisibility(prevState => ({...prevState, [field]: !prevState[field]}));
    };

    return (
        <Grid textAlign='center' style={{ height: '100vh' }}>
        <Grid.Column style={{ maxWidth: 450 }}>

            <header className="header">
                <nav className="nav">
                    <a href="#" className="nav_logo">SoftwareHouse</a>
                    <ul className="nav_items">
                        <li className="nav_item">
                            <a href="#" className="nav_link">Home</a>
                            <Link to="/orders" className="nav_link">Orders</Link>
                            <Link to="/settings" className="nav_link">Settings</Link>
                            <Link to="/users" className="nav_link">Users</Link>
                        </li>
                    </ul>
                    <button className="button" id="form-open" onClick={toggleForm}>Login</button>
                </nav>
            </header>
            <section className={`home ${isFormOpen ? "show" : ""}`}>
                <div className="form_container">
                    <i className="uil uil-times form_close" onClick={toggleForm}></i>
                    <div className={`form ${isSignUpActive ? "active" : ""}`}>
                        <form action="#">
                            <h2>Login</h2>
                            <div className="input_box">
                                <input type="email" placeholder="Enter your email" required/>
                                <i className="uil uil-envelope-alt email"></i>
                            </div>
                            <div className="input_box">
                                <input
                                    type={passwordVisibility.login ? "text" : "password"}
                                    placeholder="Enter your password"
                                    required
                                />
                                <i className="uil uil-lock password"></i>
                                <i
                                    className={`uil ${passwordVisibility.login ? "uil-eye" : "uil-eye-slash"} pw_hide`}
                                    onClick={() => togglePasswordVisibility('login')}
                                ></i>
                            </div>
                            <div className="option_field">
                <span className="checkbox">
                  <input type="checkbox" id="check"/>
                  <label htmlFor="check">Remember me</label>
                </span>
                                <a href="#" className="forgot_pw">Forgot password?</a>
                            </div>
                            <button className="button">
                                <img src="./google-logo.jpeg" style={{width: '20px', height: '20px'}}/>
                                Sign in with Google
                            </button>
                            <div className="login_signup">Don't have an account? <a href="#" id="signup" onClick={switchForm}>Signup</a></div>
                        </form>
                    </div>
                    <div className={`form signup_form ${!isSignUpActive ? "active" : ""}`}>
                        <form action="#">
                            <h2>Signup</h2>
                            <div className="input_box">
                                <input type="email" placeholder="Enter your email" required/>
                                <i className="uil uil-envelope-alt email"></i>
                            </div>
                            <div className="input_box">
                                <input
                                    type={passwordVisibility.signup ? "text" : "password"}
                                    placeholder="Create password"
                                    required
                                />
                                <i className="uil uil-lock password"></i>
                                <i
                                    className={`uil ${passwordVisibility.signup ? "uil-eye" : "uil-eye-slash"} pw_hide`}
                                    onClick={() => togglePasswordVisibility('signup')}
                                ></i>
                            </div>
                            <div className="input_box">
                                <input
                                    type={passwordVisibility.confirm ? "text" : "password"}
                                    placeholder="Confirm password"
                                    required
                                />
                                <i className="uil uil-lock password"></i>
                                <i
                                    className={`uil ${passwordVisibility.confirm ? "uil-eye" : "uil-eye-slash"} pw_hide`}
                                    onClick={() => togglePasswordVisibility('confirm')}
                                ></i>
                            </div>
                            <button className="button">Signup Now</button>
                            <div className="login_signup">Already have an account? <a href="#" id="login" onClick={switchForm}>Login</a></div>
                        </form>
                    </div>
                </div>
            </section>
        </Grid.Column>
            </Grid>
    );
}

export default Login;

