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
	auto messageWorking = message;

	for (auto i = 0U; i < message.size(); ++i)
	{
		const auto divisionRes = Divide(messageWorking, controlPolinom);
		if (CalculateOnes(divisionRes) <= maxFixingErrors)
		{
			messageWorking = Add(divisionRes, messageWorking);
			for (auto j = 0U; j < i; ++j)
			{
				RightShift(messageWorking);
			}

			message = messageWorking;
			return true;
		}

		LeftShift(messageWorking);
	}

	return false;
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
	auto num = StringToULong(message);
	num <<= 1;
	num |= ((num >> message.size()) & 1);

	message = ULongToString(num, message.size());
}

void RightShift(std::string& message)
{
	auto num = StringToULong(message);
	num |= ((num & 1) << message.size());
	num >>= 1;

	message = ULongToString(num, message.size());
}

std::string Add(const std::string& a1, const std::string& a2)
{
	const auto a1ul = StringToULong(a1);
	const auto a2ul = StringToULong(a2);

	return ULongToString(a1ul ^ a2ul, std::max(a1.size(), a2.size()));
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

std::string ULongToString(const unsigned long num, const int stringLen)
{
	std::string res;
	for (int i = 0; i < stringLen; ++i)
	{
		res.push_back(BoolToChar(num & (1 << i)));
	}
	return res;
}

unsigned long StringToULong(const std::string& num)
{
	unsigned long res = 0;
	for (int i = num.size() - 1; i >= 0; --i)
	{
		res <<= 1;
		res |= static_cast<unsigned long>(CharToBool(num[i]));
	}

	return res;
}
