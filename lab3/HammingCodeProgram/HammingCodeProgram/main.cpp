#include <exception>
#include <iostream>

int main(int argc, char* argv[])
{
	int errorCode = 0;

	try
	{
		std::cout << "Hamming code example application was started" << std::endl;

		//TODO: write code

		std::cout << "Application was successfully finished" << std::endl;
	}
	catch(std::exception e)
	{
		std::cout << "We have an error: " << e.what() << std::endl;
		errorCode = 2;
	}
	catch (...)
	{
		std::cout << "We have an unknown error" << std::endl;
		errorCode = 1;
	}

	std::cout << "Exiting with error code " << errorCode << "..." << std::endl;
	return errorCode;
}
