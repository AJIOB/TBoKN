#include <exception>
#include <iostream>

int main(int argc, char* argv[])
{
	const int messageLengthInBytes = 2;
	const int bitsInByte = 8;
	const int numOfControlBits = 5;

	int errorCode = 0;

	try
	{
		std::cout << "Hamming code example application was started" << std::endl;

		if (argc != 2)
		{
			throw std::exception("Need to pass only one argument: message to encode");
		}

		std::string inputMessage(argv[1]);
		if (inputMessage.length() != messageLengthInBytes)
		{
			throw std::exception("First argument must contains 2 symbols only");
		}

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
