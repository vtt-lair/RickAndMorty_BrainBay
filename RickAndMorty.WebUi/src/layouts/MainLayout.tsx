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
import { Outlet, useNavigate, useLocation } from 'react-router';
import Logo from '../components/logo/Logo';
import { useTranslation } from 'react-i18next';

const pages = ['menu.character_list', 'menu.add_character'];

export default function MainLayout() {
    const { t } = useTranslation();
    const [anchorElNav, setAnchorElNav] = useState<null | HTMLElement>(null);
    const navigate = useNavigate();
    const location = useLocation();

    const navigateTo = (path: string) => {
        navigate(path);
    };

    const isActiveRoute = (page: string) => {
        const routePath = `/${page.toLowerCase().replace('menu.', '').replace('_', '-')}`;
        return location.pathname === routePath || 
               (page === 'menu.character_list' && location.pathname === '/');
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

                            <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }} alignItems="center" justifyContent="flex-end">
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
                                        <MenuItem 
                                            key={page} 
                                            onClick={() => { handleCloseNavMenu(); navigateTo(`/${page.toLowerCase().replace('menu.', '').replace('_', '-')}`);}}
                                            sx={{ 
                                                my: 0,
                                                px: 2,
                                                py: 1,
                                                display: 'block',
                                                backgroundColor: isActiveRoute(page) ? 'rgba(25, 118, 210, 0.12)' : 'transparent',
                                                borderLeft: isActiveRoute(page) ? '4px solid #1976d2' : '4px solid transparent',
                                                '&:hover': {
                                                    backgroundColor: isActiveRoute(page) ? 'rgba(25, 118, 210, 0.2)' : 'rgba(0, 0, 0, 0.04)',
                                                },
                                            }}
                                        >
                                            <Typography 
                                                sx={{ 
                                                    textAlign: 'left',
                                                    fontWeight: isActiveRoute(page) ? 'bold' : 'normal',
                                                    color: isActiveRoute(page) ? '#1976d2' : 'inherit',
                                                }}
                                            >
                                                {t(page)}
                                            </Typography>
                                        </MenuItem>
                                    ))}
                                </Menu>
                            </Box>                            

                            <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }} alignItems="center" justifyContent="flex-end">
                                {pages.map((page) => (
                                    <Button
                                        key={page}
                                        onClick={() => navigateTo(`/${page.toLowerCase().replace('menu.', '').replace('_', '-')}`)}
                                        sx={{ 
                                            my: 2, 
                                            color: 'white', 
                                            display: 'block',
                                            backgroundColor: isActiveRoute(page) ? 'rgba(255, 255, 255, 0.2)' : 'transparent',
                                            '&:hover': {
                                                backgroundColor: isActiveRoute(page) ? 'rgba(255, 255, 255, 0.3)' : 'rgba(255, 255, 255, 0.1)',
                                            },
                                            borderBottom: isActiveRoute(page) ? '2px solid white' : 'none',
                                        }}
                                    >
                                        {t(page)}
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
