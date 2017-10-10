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
	return message;
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
		message[lastPosOfMadeError] = (message[lastPosOfMadeError] == one ? zero : one);
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
