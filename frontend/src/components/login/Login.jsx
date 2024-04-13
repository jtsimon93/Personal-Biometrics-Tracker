import { Link } from "react-router-dom";
import white_logo from "../../assets/images/logo-white.svg";
import logo from "../../assets/images/logo.svg";
const Login = () => {
  return (
    <div className="flex h-screen bg-gray-100 align-middle">
      <div className="h-full w-1/2 flex-col justify-center bg-[#FF5000] text-white max-md:hidden md:flex">
        <div className="flex w-full flex-col items-center p-4">
          <img
            src={white_logo}
            width={250}
            height={50}
            alt="OpenDiet logo"
            className="mb-2"
          />
          <h2 className="text-2xl font-semibold">
            Empower your health journey with personal biometric insights
          </h2>
        </div>
      </div>

      <div className="flex h-full flex-col justify-center p-2 max-md:w-full md:w-1/2">
        <div className="mx-auto flex max-w-md flex-col">
          <div className="mb-2 w-full justify-center max-md:flex md:hidden">
            <img src={logo} alt="OpenDiet Logo" width={250} height={50} />
          </div>
          <h2 className="mb-5 text-3xl font-semibold">Log into your account</h2>
          <div className="mb-2 flex flex-col">
            <label htmlFor="username" className="mb-1">
              Username
            </label>
            <input
              className="rounded-lg border border-gray-400 p-2"
              type="text"
              id="username"
              name="username"
              required
            />
          </div>
          <div className="mb-2 flex flex-col">
            <label htmlFor="password" className="mb-1">
              Password
            </label>
            <input
              className="rounded-lg border border-gray-400 p-2"
              type="password"
              id="password"
              name="password"
              required
            />
          </div>
          <div className="mb-2 mt-2 flex flex-row justify-between">
            <div className="flex flex-row gap-2">
              <input
                type="checkbox"
                name="remember"
                id="remember"
                defaultChecked
              />
              <label htmlFor="remember">Remember me</label>
            </div>
            <Link to="/" className="underline">
              Forgot your password?
            </Link>
          </div>
          <button
            type="submit"
            className="mt-2 rounded-lg bg-[#ff5000] p-2 font-bold text-white hover:bg-[#ff4000] active:bg-[#ff3800]"
          >
            LOGIN
          </button>
          <div className="mt-2 w-full text-center">
            <Link to="/registration" className="text-sm underline">
              Register as a new user!
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
