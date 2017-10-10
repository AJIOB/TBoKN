#include <bitset>
#include <exception>
#include <random>
#include <chrono>

#include "MyLib.h"

const int messageLengthInBits = 4;
const int numOfControlBits = 3;
const std::bitset<messageLengthInBits> controlPolinom("1011");
const int maxErrors = 1;

std::string EncodeMessage(std::string message)
{
	return message;
}

std::string DecodeMessage(std::string message)
{
	return message;
}

std::string MakeErrors(std::string message, int& lastPosOfMadeError)
{
	return message;
}

bool FindAndRemoveErrorsIfCan(std::string& message)
{
	return true;
}

std::string Divide(std::string dividiend, std::string divisior)
{
	return dividiend;
}

void CheckMessageLength(const std::string& message, const int requiredLength)
{
	if (message.length() != requiredLength)
	{
		throw std::exception("Bad message length");
	}
}

