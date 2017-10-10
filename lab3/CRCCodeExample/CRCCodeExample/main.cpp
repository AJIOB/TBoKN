#include <exception>
#include <iostream>
#include <string>

#include "MyLib.h"

int main(int argc, char* argv[])
{
	int errorCode = 0;

	try
	{
		std::cout << "CRC code example application was started" << std::endl;

		if (argc != 2)
		{
			throw std::exception("Need to pass only one argument: message to encode");
		}

		const std::string inputMessage(argv[1]);
		std::cout << "Original message: " << inputMessage << std::endl;

		const auto encoded = EncodeMessage(inputMessage);
		std::cout << "Encoded message: " << std::endl << encoded << std::endl;

		int errorOriginalPos = 0;

		auto withErrors = MakeErrors(encoded, errorOriginalPos);
		std::cout << "Message with error:" << std::endl << withErrors << std::endl;
		std::cout << "Error original position " << errorOriginalPos << std::endl;

		const auto isCanFixErrors = FindAndRemoveErrorsIfCan(withErrors);
		std::cout << "We can" << (isCanFixErrors ? "" : "not") << " fix errors" << std::endl;
		if (isCanFixErrors)
		{
			std::cout << "Message without error:" << std::endl << withErrors << std::endl;
		}

		const auto newMessage = DecodeMessage(withErrors);
		std::cout << "Decoded message: " << newMessage << std::endl;

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
