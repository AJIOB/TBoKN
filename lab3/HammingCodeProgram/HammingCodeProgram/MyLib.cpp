#include <bitset>

#include "MyLib.h"

const int messageLengthInBytes = 2;
const int bitsInByte = 8;
const int numOfControlBits = 5;
const int maxErrors = 1;

std::deque<bool> EncodeMessage(std::string message)
{
	if (message.length() != messageLengthInBytes)
	{
		throw std::exception("First argument must contains 2 symbols only");
	}

	std::deque<bool> result = BytesToBits(message);

	//TODO

	return result;
}

std::string DecodeMessage(std::deque<bool> message, int& numOfFoundErrors, int& posOfError)
{
	//TODO
	return std::string();
}

std::deque<bool> MakeErrors(std::deque<bool> bits, int& posOfMadeError)
{
	//TODO
	return std::deque<bool>();
}

std::deque<bool> BytesToBits(std::string bytes)
{
	std::deque<bool> result;
	for (auto c : bytes)
	{
		std::bitset<bitsInByte> bitset(c);
		for (auto i = 0U; i < bitset.size(); ++i)
		{
			result.push_back(bitset[i]);
		}
	}

	return result;
}

std::string BitsToBytes(std::deque<bool> bits)
{
	std::string result;

	std::bitset<bitsInByte> bitset;
	int bitPos = 0;
	for (auto b : bits)
	{
		bitset[bitPos] = b;
		bitPos++;
		if (bitPos >= bitsInByte)
		{
			result.push_back(static_cast<const char>(bitset.to_ulong()));
			bitPos = 0;
			bitset.reset();
		}
	}

	if (bitPos > 0)
	{
		result.push_back(static_cast<const char>(bitset.to_ulong()));
	}

	return result;
}

std::ostream& ShowDequeBools(std::ostream& os, const std::deque<bool>& bits)
{
	//TODO
	return os;
}
