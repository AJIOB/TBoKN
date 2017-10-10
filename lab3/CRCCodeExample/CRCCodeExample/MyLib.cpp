#include <bitset>
#include <exception>
#include <random>
#include <chrono>

#include "MyLib.h"

const int messageLengthInBits = 4;
const int numOfControlBits = 3;
const int polinomLength = 4;
const std::string controlPolinom("1011");
const int maxErrors = 1;
const int maxFixingErrors = 1;
const char zero = '0';
const char one = '1';

std::string EncodeMessage(std::string message)
{
	CheckMessage(message, messageLengthInBits);

	auto messageToDivide = message;

	//G(x) * x^3
	for (int i = 0; i < numOfControlBits; ++i)
	{
		messageToDivide.push_back(zero);
	}

	//G(x) * x^3 + R(x)
	message += Divide(messageToDivide, controlPolinom);

	return message;
}

std::string DecodeMessage(std::string message)
{
	CheckMessage(message, messageLengthInBits + numOfControlBits);
	return message.substr(0, messageLengthInBits);
}

std::string MakeErrors(std::string message, int& lastPosOfMadeError)
{
	CheckMessage(message, message.length());

	// configuring random generator
	const auto seed = std::chrono::system_clock::now().time_since_epoch().count();
	std::default_random_engine generator(static_cast<unsigned int> (seed));
	const std::uniform_int_distribution<int> distribution(0, message.size() - 1);

	for (auto i = 0U; i < maxErrors; ++i)
	{
		lastPosOfMadeError = distribution(generator);
		message[lastPosOfMadeError] = BoolToChar(!CharToBool(message[lastPosOfMadeError]));
	}

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

void CheckMessage(const std::string& message, const int requiredLength)
{
	CheckMessageLength(message, requiredLength);

	if (std::find_if(message.begin(), message.end(), [](char c) -> bool {
		return (c != zero && c != one);
	}) != message.end())
	{
		throw std::exception("Bad message containing");
	}
}

int CalculateOnes(const std::string& message)
{
	return std::count(message.begin(), message.end(), one);
}

void LeftShift(std::string& message)
{
	//TODO
}

void RightShift(std::string& message)
{
	//TODO
}

std::string Add(const std::string& a1, const std::string& a2)
{
	//TODO
	std::string res(std::max(a1, a2), zero);

	for (auto i = res.size() - 1; i >= 0; --i)
	{
		
	}

	return a1;
}

bool CharToBool(const char c)
{
	if (c != one && c != zero)
	{
		throw std::exception("Bad input for CharToBool function");
	}

	return (c == one);
}

char BoolToChar(const bool c)
{
	return (c ? one : zero);
}
