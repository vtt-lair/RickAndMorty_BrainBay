import './App.css'
import { RouterProvider } from 'react-router'
import router from './Router'
import { SnackbarProvider } from 'notistack'
import { SnackbarUtilsConfigurator } from './components/snackbar-utils/SnackbarUtils'

function App() {
  return (
    <SnackbarProvider
      maxSnack={3}
      anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      autoHideDuration={3000}
      preventDuplicate
    >
      <SnackbarUtilsConfigurator />
      <RouterProvider router={router} />
    </SnackbarProvider>
  )
}

export default App
