import {Container, Menu, Image} from "semantic-ui-react";
import {NavLink} from "react-router-dom";
import "./Navbar.css";

function Navbar() {


    return (
        <Menu fixed='top' inverted className='navbar'>
            <Container>
                <Menu.Item as='a' header>
                    <Image size='mini' src='/vite.svg' style={{ marginRight: '1.5em' }} />
                    UpStorage
                </Menu.Item>
                <Menu.Item as={NavLink} to="/" className='navbar-item'>Home</Menu.Item>
                <Menu.Item as={NavLink} to="/accounts" className='navbar-item'>Accounts </Menu.Item>
                <Menu.Item as={NavLink} to="/countries" className='navbar-item'>Countries </Menu.Item>
                <Menu.Item as={NavLink} to="/dafasdqweasdaf" className='navbar-item'>Not Found</Menu.Item>
            </Container>
        </Menu>
    );
}

export default Navbar;
