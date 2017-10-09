#include "MyLib.h"

const int messageLengthInBytes = 2;
const int bitsInByte = 8;
const int numOfControlBits = 5;


std::deque<bool> EncodeMessage(std::string message)
{
	if (message.length() != messageLengthInBytes)
	{
		throw std::exception("First argument must contains 2 symbols only");
	}

	std::deque<bool> result;

	//TODO

	return result;
}

std::string DecodeMessage(std::deque<bool> message, int& numOfFoundErrors)
{
	//TODO
	return std::string();
}
