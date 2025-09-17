// import { useTranslation } from 'react-i18next';
import { 
    AppBar,
    Box,
    Button,
    Container,
    IconButton,
    Menu,
    MenuItem,
    Toolbar,
    Typography,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import { useState } from 'react';
import { Outlet, useNavigate } from 'react-router';
import Logo from '../components/logo/Logo';

const pages = ['Character List', 'Add Character'];

export default function MainLayout() {
    // const { t, i18n } = useTranslation();
    const [anchorElNav, setAnchorElNav] = useState<null | HTMLElement>(null);
    const navigate = useNavigate();

    const navigateTo = (path: string) => {
        navigate(path);
    };

    const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorElNav(event.currentTarget);
    };

    const handleCloseNavMenu = () => {
        setAnchorElNav(null);
    };

    return (
        <>
            <header>
                <AppBar position="static">
                    <Container maxWidth="xl">
                        <Toolbar disableGutters>
                            <Logo />

                            <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
                                <IconButton
                                    size="large"
                                    aria-label="account of current user"
                                    aria-controls="menu-appbar"
                                    aria-haspopup="true"
                                    onClick={handleOpenNavMenu}
                                    color="inherit"
                                >
                                    <MenuIcon />
                                </IconButton>
                                <Menu
                                    id="menu-appbar"
                                    anchorEl={anchorElNav}
                                    anchorOrigin={{
                                        vertical: 'bottom',
                                        horizontal: 'left',
                                    }}
                                    keepMounted
                                    transformOrigin={{
                                        vertical: 'top',
                                        horizontal: 'left',
                                    }}
                                    open={Boolean(anchorElNav)}
                                    onClose={handleCloseNavMenu}
                                    sx={{ display: { xs: 'block', md: 'none' } }}
                                    >
                                    {pages.map((page) => (
                                        <MenuItem key={page} onClick={handleCloseNavMenu}>
                                            <Typography sx={{ textAlign: 'center' }}>{`${page}`}</Typography>
                                        </MenuItem>
                                    ))}
                                </Menu>
                            </Box>                            

                            <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
                                {pages.map((page) => (
                                <Button
                                    key={page}
                                    onClick={() => navigateTo(`/${page.toLowerCase().replace(' ', '-')}`)}
                                    sx={{ my: 2, color: 'white', display: 'block' }}
                                >
                                    {page}
                                </Button>
                                ))}
                            </Box>                        
                        </Toolbar>
                    </Container>
                </AppBar>
            </header>
            <main>
                <Outlet />
            </main>
        </>
    )
}
