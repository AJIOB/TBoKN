#include <exception>
#include <iostream>
#include <string>

#include "MyLib.h"

int main(int argc, char* argv[])
{
	int errorCode = 0;

	try
	{
		std::cout << "Hamming code example application was started" << std::endl;

		if (argc != 2)
		{
			throw std::exception("Need to pass only one argument: message to encode");
		}

		const std::string inputMessage(argv[1]);
		std::cout << "Original message: " << inputMessage << std::endl;

		std::cout << "Encoded message:" << std::endl;
		const auto encoded = EncodeMessage(inputMessage);
		ShowDequeBools(std::cout, encoded);

		int errorOriginalPos = 0;

		std::cout << "Message with error:" << std::endl;
		const auto withErrors = MakeErrors(encoded, errorOriginalPos);
		ShowDequeBools(std::cout, encoded);

		std::cout << "Error original position " << errorOriginalPos << std::endl;

		int errorsFound = 0;
		int errorPos = 0;
		std::cout << "Decoded message:" << std::endl;
		const auto newMessage = DecodeMessage(withErrors, errorsFound, errorPos);
		ShowDequeBools(std::cout, encoded);

		std::cout << "Found " << errorsFound << " errors on position " << errorPos << std::endl;

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
