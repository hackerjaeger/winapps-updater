﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.1.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "c7f21d09314bc15dc16be60da7eb8a1f271bb6a5914d86d06953cae646f0db6859d61a4fe4d887ac76b9c38708dca71057bde20eb3ce2bdd0f0f50bd713ceb43" },
                { "ar", "f8c42c8c78d282fcab7d0ab771e99e8631fac9143baeccccc6772dc165db7d7d1adbc63bf18afb09c5040026ec78d376e9683cec1254c9b19628873d38a6f1be" },
                { "ast", "9861150295ade9228a35755d1a01293bd823976624c9e3785ffc978a1d9931a48d0c54cf15acc9bba6a4ef8a594a0620e65df94e5ccd9f741083b1fac64ce1c8" },
                { "be", "69052ee8cdf13003479809d504fdc203e3416eb3b0dba2e66341fd243e3b12618bb6d5ba52f7ed618c757963e4ec9ef9c7a59c2de819101d60df9b7ffe0532dd" },
                { "bg", "2a2270642b5caf292df2500c76ddc8f3d45168fb513b49535112b00e7b8e2f9e6018db1f4f09e6f1aa9b2024f7e62f750467ae63ec75b0a42eb6a01a2ed99607" },
                { "br", "43a81394cef203512b6b506a9cbd2fdfe70b113830da65cbd3860fe487e02973b14014dcaaf7283f8f6ebd6afe22882797a089908f0add4af4423dbd891537ea" },
                { "ca", "547d5672363f372b97afea7145e752908dbb05c76a010782dda83902430b7a8d1e5955487850128e09137fd78c8026cd005dde62cbb8e780c5c1a5ab793e98a5" },
                { "cak", "49a7f88dd6cd8278e0f2dbc75cf13d94e1fb17cd6d27ed64dd2d2d4d3da88885f0328622479cc6c0fddc66344d215a88ea7588394a62ef5f360014b692e90287" },
                { "cs", "24f7ca5d4d35eb2fbd3013ee1d886cc21ddd5715db5640b4cd016916e9a7d6000ab71f95fff94561c79e46518797e79ac0eefe3d01602dbed5c6b98132826bb2" },
                { "cy", "a90c42cea5a109054a931128db886cc6fa0f7e284ec4dfc31e8fd410faa37c5f1b25559831f80377c7999b38055cf9e97498cb157d8273cfd1cc740e266b0c07" },
                { "da", "62fddc01c2fd41c07076446442378bbc49a47e6bf1dfd64feccf54d734b7c502032bdb427a3543de381384403128d377a39bf84983da9c29109b0bf9262535bb" },
                { "de", "38b7a6d55bc054a70e148f45d50624924ef2f16ec98e87427f9ef409425718b0e10c0e386a931b98ba043fb2eaa4d8d63db93691c1599435862a335db6e48de3" },
                { "dsb", "15ba1aa27df84cf11d2fa5b6c428b0501c0ab5452b69db599917c074d9183e83aff8df9afaed80a3a9e8e1c957adb40f92485a5c2bac9dd496145bbdf3c522b7" },
                { "el", "afe01b2c3c452d909723331b361f7f57f81b0350efdc94a26798f74101dcca0d07b48578dde089cb5eb1d0a232585bea32884f8a01453c134591b546745d08fc" },
                { "en-CA", "c2c679759ce8b34e852cd5b0badc0043ec268b8125bf93d75ec78c927ffa7fd04ed1b9da2cc5802bd177b18be1984a81b04f081c9ce6c3568ca84a4aa3747630" },
                { "en-GB", "40e8d3c2f08a67ae47c91a9a1cc30c9871e0c9cffe3ca295efeb4dacc83e8d5c4cecde1730342cd1fe633cc5b7871a359f0cb7a9a1a2bc287c7ae3f16432a4ae" },
                { "en-US", "474d47b152ebb1922487821aecbabfcecc60c77d29262dc90e4e37eb98e755b0cd2faafd11f441b21ae1fb3b146910612bc7adb8635d3dc27766f25102076364" },
                { "es-AR", "53097a6ddf06eb9294baff34129a6408777b934db68ba5df4f43ba90e76902437664fd733b5762946905be10e812953dcf7dbc478e506b499332c105d14158c6" },
                { "es-ES", "e7824aa490fe20bd5a55f616b1736aa39e8d1a3f1f4d51179be8181eae6673f0c92147b4882eb485759acc99e078a05bfa6754b2f01dfa2b6564ea510744b6f1" },
                { "es-MX", "114e2ff495bbae24eec22e318aa67bdb79ef555511873d1db47de04244fb69734400db6f9ec2b348d10b6684ab95a2ac7f176464588f41d3296d69f7121ec41f" },
                { "et", "6e38e7686f6c5f1ef255a78a98acdf5c8dad8498ef55ed7e972dec77b0a9a3d3558656fb5f25ac95e72533305453e0eba476b63d92d3b28b04dc5dab21c888e6" },
                { "eu", "3e47bde096d17997e4065f853aa1364df48fdbce8d057a4793cfae1e5941b424015e9cf579f521485ccb816f4130660fcd545700dfc8af4a5184710e1ef20ce3" },
                { "fi", "fb17b2290f05ee8361a853371cac11bc92dfb2b17806696f91e263fbf05a3d18581bb213babd17db77ea91a1eeb76d36c54fae4a1c947d6728e04ab9f409c3b7" },
                { "fr", "4d53d6a641013fc96f308692ce2f5139c3af218075f45bf5a3df60da3d085da4abb615611fa7db46c3fd13d4db28929563cfd37d67c18786e2fa639298966192" },
                { "fy-NL", "0b267d15d3343439939097e46cb8ca27ff53f295aa0973c7e6831128e75607ecf2e488da685a7ab91cf1a140b130971a6cb0f1c8dd55359ef7ad90f256b71b2e" },
                { "ga-IE", "77e4201c1e789dd4f6189c6fbb3182ddaf092e06eed5ed92d6bb8a99c5456b7614ed86d40f2966cc7a1aca25cbab641b283e3d9264f979279895a9d892f119ca" },
                { "gd", "f9164a9acf461b57c00065ffb680211ad48bb1bef8956fb1377f9880e04a090d755e98aad4d133f7c0c806baa7a2f19bcd16547274c92db0b7cb60dba4034c26" },
                { "gl", "8a30998674661ead27d9930b732f65cf04d0c0cf55e7100a263d4b16ebf004ee159e364b4af4ef308f21e3f0d990096d1c3e6b5ca5aff75d59818ab6926711d7" },
                { "he", "6f36c5823bdffaa82c57098e91fb5563136bbb007218df7029df63edd23b8b6c17ecb92a1bdc292ece2fc520609ae78faf5c04706589c8e046396f8ec1f140ac" },
                { "hr", "8d2a84ddea7d8100f04980a91f4b6faf1ad2d8df3e4ccc5387c4e5c51b151503c19a7777184c63fe1249b592ac87291f01a7acc38c772becc2f48e4b826acaf1" },
                { "hsb", "4ca39da830d89ff2c73ec4695d441b2ceea83581d921c94df9abd181021a6cd84291965d15222453478b83c0a9a10d1c0cdf78d3906b14d4943d54341e07cd39" },
                { "hu", "f1b83eb4d2cee6f75ea1dfec7fb752c8ab2ad185dfb4b053130b1f03a5e83699d57c3807861855bebfe94ea2beaee79c226cde69af3a767c989dc000d3bb1f21" },
                { "hy-AM", "fb12da67f1369dc9ca968e6651a6212c87602c6084b560c37f93f2a694dda03ef60ca02fc59542458b0308b1b8948e7c8cb78c0ec9c988fbe88a574ec5e53592" },
                { "id", "a7ac332757c48bca2430942a88a96f7eae3f8c4902dbbe3f49534e2a2d4ce47d58094634500e99f0f9c1fc69483a9ffeb6730b50dcf9408f4790dd046fea1c17" },
                { "is", "6bec95f54ec53f30674047308961418dbf46a76a1f6d89b0eae8eb4c8050263bf76836e08d41042206a3402845c4391589df9b9ece235d6ff345872c98761f74" },
                { "it", "482ce1f613c7d16d344f6248a2ae212accc3c87342a68d8a3e74de31818458fd7a592fb0943d423d8978e2b6ead35ee42fff31f9dd2a38a777529cf0d54ef56b" },
                { "ja", "2433a1d1083379a53709dbfa012758497c940d8a3510af1fb75f30ede38c0d05ab5b2345ce93db63341db04b54791551d604475c322103fb6f94ab871374d3f8" },
                { "ka", "6e2cc546b1158374c55512fdaa0f171e45ba4fc73561c5e6914c90531dcd207115e6f12feac842888ab88719a4b988aaa4da4019d9a10f298af051f4f2b68895" },
                { "kab", "d8938ebba17ec77eaf799acaaa2f4212e1c9065e6b432f0d83f2e5334a4e73e9df5f87a340b97ba03e8298ca5864380db5eb07e764f8370bdfdf222e09abffdb" },
                { "kk", "2846906e48e64297aa6abed656c4f125251e711f7f84fd2ed0ba831d11556503fa3228a6052d2bb5b74d8f652c3727a627b0021a856b4a81cc869cded6a85db8" },
                { "ko", "c989777e21dd33b9cb3fe9047841b9757753c535c1c765dea74ab6b42509ed011089da4c041a9918cb97d85186139911b34cfc42a584dbdbc94605d6b70a0c91" },
                { "lt", "e3bafdd02d6f67770d79450090c0fe7757a36aa82931c6126af75d37881dd7f89a54eb88e0968b8809942782e4ff626ebd87c2616342bc4603d9d5c317739c75" },
                { "lv", "72742f02d42669f1e93f8f7275eaae959639fd4705983709607997516d00399e10bc1414a97b7d6b2c12d6665adbe77cc6ccaabdfbf5a8c89f202fb95387ed9c" },
                { "ms", "3fa33951489122e440b31132310d9425970c4b5f883fd77d0a7cf85dc52c1228bfb49c0715f77d3256665f984e74651df82340d099e2e5ea769bff7622776e97" },
                { "nb-NO", "908bc7e29505592226eee2f96c7a4da87c54e8f3df6b4af63a8f696d0255b80ba4898274016e32964be118e5c092cf44329df2e3539895cdf0a1180c89c5e2b5" },
                { "nl", "7d288ee4cb0982e8cdccbdde5d7c8ff56e2ccd0144c131e142454a3e9bda922d5a7c670824bd5a5aa038cc1b413563dffe45897a739a328e327299f7730a2ce8" },
                { "nn-NO", "73c8ac35875e5c572fb035b2cdd7e31d7191741071cad7b9488d139bdd49afe147018058af1a92872ed4397b66c2d448669a5952d816e1d485330fc11682cec2" },
                { "pa-IN", "3a180dea058425bbc5ed130f2e779267275a59772d330016a3e6da44f1dafceb284ec47c24c74e23a5dd170f29dfec8571dcf78cb176ae235e156d25ee1c89b9" },
                { "pl", "66e651095361696256b2f7e9cf0a8ec77f433285603e837581ab87710adec8bd4ca00a14621b51b33bf259f340e5ed50e497550b3e36af5976023646ab1be93f" },
                { "pt-BR", "a221a67ef5245d6be1fdb6f28dc36597f5bcf419821b29c7c07c0dd43dfa495e3344b51a3c66df8a867f458b95e9f43d0fdb9a9deb446301d20fe2770b635a6c" },
                { "pt-PT", "e6d3e4f9f2e08fb70c19ebfd8ed4dc359642063f8d96edff5225d17ba206021439b00b27a4acfa2cac342f8b4234822a6c7a44a5dcff18ac0747ad0c4b5ef49d" },
                { "rm", "3a8cd2aee7c1ba322c39746a26d2bc06e35b200553dc2b8fd87c0da8f80d9d979094aa50d3a51e8b57c2cb4d9ccde170446212ee52af6b00018cb18ad85b95c9" },
                { "ro", "ca26784287b778cd5338199f98b97aa541899dc145ae422f112cfe945e853658c2e432fbe51b26b16d55e70268479c1b80c0e568af5b96640378124242cf678b" },
                { "ru", "826c54bfe9c6df94776bf37cf5c4567bcfa81001a889b46474ae0be7f1b60a7e90ea113724939fef7563e77f1ab28361fca62d95e6ee7d73a042e8361b7b5299" },
                { "sk", "8c1fa40eeb2cdcc0b3e0453b5073e52499756dab5d998dbb806ff7a3bb0529c0f92e7ff8c8a0f95844c5c6ae7c0568c9ba91bfafa6b85a484e579f80f5757697" },
                { "sl", "3967ab58727b77e5ee34104711e7010b2a17947492993707102fb162598e2dbd52c7be2e521f44bdddafb0f2941e3eb5872af10fbbfc2ebd5224e6e515503a78" },
                { "sq", "67a784d14080801c51d1cae2acf011acf9183d65db567839d0467ea5e36cb72384dfa6ea4c9d1bafa5a5d0ee6e17bc8c941b5ba5d84ce945aa8b725971de71c9" },
                { "sr", "6b1a91651976ab50d5ed5939d23e92776d2f102808fea14c2407533901e3d7f310d49371b7f718fdd8653a55e0498c2b19c8a7e8f208ffe9390b78e7e3a59c3e" },
                { "sv-SE", "ec2be11023cd58c2bf227f01dad6a6fea1725a8633fb217484c3cb11abf5af48d7a0e3133461aa69b55ce49630891e5c993539c121ce963890b8fbb8b810909e" },
                { "th", "e68013a03fb623f4d9309b65bf009c46184a5fb729ab3571a847360c3dd01f8298056a11f505e1e5bc303f27287f662dccbde4d7113b0fda4d841c9810adc9c2" },
                { "tr", "a45ef97bbc089a18e94e920abac084ff017fea0c5f31ab367fee27ecdf6b9e9b3f9559733ad009ba5d3b294c1a54bd098abad1990992f3bb3949362da7a74475" },
                { "uk", "3ceef550bbd9ffc5e4264d57d4bcfb5a773958537e20472636a4ac839dd540b91f8a8a0aaef01002f49cd0cfd63fcb7e754d5609c6225df6aded2d2b4f5e7da1" },
                { "uz", "e029c03188bc06bf752f6b0ff87f058a6a9b3fd3f8a2fbba25076c987c3a5d4d3308b223442ae88036ec374fa2736e2ef6e2afba14d752e8a9867074afd87739" },
                { "vi", "dedf47245f67032f8f39aaf2af7160b8c92afe5922b47f64dd91b05a54b74a9faa8895cf10889731bbe625129a19c2f787dd20098edec260c6777bdeb40c7923" },
                { "zh-CN", "f0015325aaa8d8451a4cc1d6c3c4afe0d0c01d211dcf45342fe6397a63805916e47d36f248f764e5e0a1d043bd1cc8080a82135dce3ee116a8e55c7c26b3bed3" },
                { "zh-TW", "9a149bfab5861f69dd13d5ba7877f70c39940e38189ed607d69fbab3e2d4b413bb62aad80039bc50dce063d37297ba0662c3ad06e058c46cecc74dc1024a4afb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.1.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "3378d00ccc959974f017c8f34d4d77fb4cf53e42563f1cf5173bf249fff9039ee4ee456fa74364ba15c0699861faa5e40388d5392be9f8d391c5a3298d7d7352" },
                { "ar", "7c20fe64fb06a8caf1396b6770f7fcaeb77411e7308a8113808938e5780d83b9fbe0cd6df1aaa453ffa5c5dbf57926e468284e18526d1724e262e0ff0f022e04" },
                { "ast", "2c61abed037c783b00e0abc9dc315249ef72a4665fcfa728bb21563c9de2c4b7e634c941efca939bca66a6e1eebfe442a17ff74753dd9047e321c290bb71f45a" },
                { "be", "8ae9b8d5be29427e305c6b928245b4e78db7bae898cd637de65837319cc02cb2f62fe662bbe188860e792253f9b455f5f6089b62d6b3cf86043c382795614666" },
                { "bg", "a1d421ba27ccd5fdd06afbf7c771acf44883d20604b358f2f11cc13c9ce43a6e3c41fe16fcbd87018b5d5d260e3c3dc5bfb9097b6f79464df082a0ecf2ae7f6c" },
                { "br", "d94a8b0a60ca8217d7336c889cfed7c8c144e280b4ef476b09ede3362bf9ebde6610ea225d80e15c76d3d406cd5f17e00216353f770c8f41834392664315f608" },
                { "ca", "7c798038fe934448b929e462b5319b5b2bef87397504d7b319225a837fa83a413e9111ce4e708e987fa7c023c2a757aa3d6c0c2ccafa6858316e26d8f9812018" },
                { "cak", "f98bfd8bc93f331128a20fd5abf86110555cc88648953060d265f8c8de87f52df461e73b2004143bc2c10310df708b461d0897b3d556e9ba9d5d4766fce54361" },
                { "cs", "fe31fa82b04aafadbfc081998f6bd10d713ea483b6351e19a06d83ba06098e1ee259b8d0db92b4f7aa3ebfad68d9ed846cddf4a6e3c653a1fe13bdfa7696726d" },
                { "cy", "42d8147e5a22d73a8c2d800d1812edc59b2bf994ee1b40054e8fe6deee9012a3189e7528e53f77b8a805ae39993b16642bc95852902d049f6883c3b7c472eadd" },
                { "da", "9db50c7ae7bf68b0ace11e07700ad4ec9b7f3317a3f53c99f2134e99247dcedb9a6735401a828e5bc3d84b702347f7cdc7b91c26a9fea673d11c9801350691f9" },
                { "de", "569ee87f4cd08cbfca11d8de7ff20296ec19092dab497d7db9c2b5e8018eb4d80dd6efc977218dc7b0fe12b2aa8f61973d1cd26d738782df4e3212ada896f736" },
                { "dsb", "4af1bc9d6ae22528b0e38b8daa4671b2d968ab15a6c96809034310a710bf2c1d9af58ccb4fdd581019121de356df7c04f02a4df67aa4229ae331570c6db82f61" },
                { "el", "204ba1cc7225418bc6a757a1fecc0d8188b852d1616acf8c80d1dab156a781c35447fcd29e8bba4430f9868e056d54ed71f8691b3ecab87e218231d289afea1e" },
                { "en-CA", "63584587a96bbfd5a70c4aba6b3330b1d10ad5d1d124583a8f1d7b6177411f6c9dd3ce0bf87533a6b22a5e7b007c9b71d715c9ee7504f52b6eac135677fde1b6" },
                { "en-GB", "45356a43da468558dafb8104aa6cf5e70d72b09595df54e379107009f0f21fca42811f428028c39d3f8f1d0a08220da441749915351f11de2b20c4bd47c8a37d" },
                { "en-US", "434b8dfed8b8c0eb846983bef8121f95dac2f9c52d74101c7bf5d54e7e71bcd3f6584e5ab2d9c1b21a082a466b86688eeba36738b9f73a34dcd76ff4e349bdf7" },
                { "es-AR", "1cda6bc68d686b66eed15bebc1010602530f825d08285e7200ad91d4fc258c5b6894cd9cf2e8dace5f15078dfb1c44ccb2f8c356fa5e64d88e73c92a05b420cb" },
                { "es-ES", "7bd15622a5737cef48dc0a3df52b5c66d4fb613358272849afc945c3116bf025389cfb9229661bd7ae5314883b0b9890c075a72c43110e21379de2b4545c6931" },
                { "es-MX", "5760ba277933f880e8f7fe7049e64632f9a0004bb12c2954b9566ef350675a0b1921a3fdc328099143e6780774e73e910d08f5aff8a4bf9e1bda851d2c340d57" },
                { "et", "b38e517f2cdef51a80ed1a3f48c04f04af8dcb902c728d04ace2784eef30dc60ba7c3fefd401a3068238af6e524140b1f46f157d6ddb72968d33bdae06fea833" },
                { "eu", "2ecfe692df8517b4c969cbb03bea1f4cef9e86fe21cda2d11dca5385cef51130c1e154b2fe85c2a78993dcddf5ce351d112a422fe7b0811fc3fc0185efeea1ad" },
                { "fi", "bceae275dd627f959514cda1620fb3afa28a61597aaa5eddb6d26ade64bc443a35401ca76df3052175f3ee081b04d4dc458641fd195d41f65f1f1409667fd576" },
                { "fr", "bc12eabc09d4a1c04d2f1a71fede00170c386488c59c555a39ea48fade7626a73ae3b04d98addcf787ec29ea0359b34bf70b4de493f6dd0d0f2e9b1f34df91f8" },
                { "fy-NL", "a8f562dda0a809ba99412cb9c5ad31cf4d8ab76e4e683921ab7bfbda232a011074ff4cd4632be0d624f8a6aa908916c06db53742f62a3f27134c2cde139b05bc" },
                { "ga-IE", "9912824c09eb4d99ce753778c55f6989edd3261db7e394b2a812659a67f9674cfb4efb08ab6c01712bad52d1b97ac4a3be3117711e8676691383d721d00a044c" },
                { "gd", "73c0f9210d5debb20613b72b68ff19b146257196b0f439d825e5e16617b76b9b01ec0317018dcbfa75234c2efba6c4108496f819f96cb3ed1b9e0c32d791702c" },
                { "gl", "490b198c5a64112e0af73cbaa10c3951eff0d0735dbd0c76b3470e2f5fb0b60ed6c72a555e80a150dab0127dce47f4f22bc97ecdb4327972a3a79d82349aca23" },
                { "he", "a1a9579d7f73926ef2854d4fd8100731fba1ecf077fc459c9f76af3950273926201233c6a050dfebb01c4a91cda5d6a3496639c296e392105bc36392505fed13" },
                { "hr", "45592d786d0f8a4fd606041e2597d99f7157f573f7c5b434d98a32f48c3318943061e843e4920bb7831c426f6147332c9d8990ff549b3fdda7a1f94fb79067cf" },
                { "hsb", "467c8692795bc1c3b79bd11aa2fe8ec58e05b3939a8f970e522be63d2a3938fd62a67a41e1f7717fa57bac968129df7b6b4f362e325df48bc18be0f825a489b2" },
                { "hu", "5b9f25324998302f34079e0180b39ee168b79d48fe2ad5704689161a7f9bb539495977e4c27e69187ba3c37deb8058ed6b24907c9423e4b46cb81c3a46468819" },
                { "hy-AM", "fd5549bb39296731d81cd0586c941d51f7d99833ef0326d02ea7e6e11d9598db83740a23ff425fcb6c2207fcdba39ddba5a79a244d849e92cfe6bd7496d99534" },
                { "id", "4548e9c1bfd1f737ae156a383bbf8ec36896d40400d6293c11a77bd8fb2a9c90db354f27f1298b6265802adb1ab77074bec9e3d4b2f32565fca1c2360a4be5a4" },
                { "is", "12c55befc81c6655a0a238e8e2644e9e5da87c83f70348ea800c48d4b119552590677f7d966b183957894b557b9077acfd2c788e68180d33d2c47bd15764dffe" },
                { "it", "2afef7da7a19c65822efc419d50d1344eb3de3bab04b61854475a6ab8c465c32a43bc653dd785a4f91e4db74c5796ea6331149dda1e61d937692fe8d31ef647c" },
                { "ja", "f92ca64a47e1943fc47b33deb2e264b861e03c4b14db7ede3b00434c6d7fda1b683a55e3fd56e7b3215b12c2e7ae3d7c2addd9f15135197a8e36e31973ef84d8" },
                { "ka", "0c5ef153611d082c018ccd541be8b4fcc564f7a1e7286ae6d26ba5e38c35d99c1aaf10e36bc43666bcb85cee6423a425df745114a56c68f42f0821d53b47ecdc" },
                { "kab", "338080ef58a126423693d55fa267a5138f9e28f764161b595a9f471a12bb7564861079dc26fed4892dcfa64a7d004ed5e37822b8ca7758266d054efd94396b6d" },
                { "kk", "bc7b5a06926931f0a786705db2c059516c30704eb3af8995ffb64fd3fe2d50d9d28829c8667d124837cbe62435a5d0e2271925dc2340dc64b62664cc51bcd43c" },
                { "ko", "a2b10c9c7022e7f20cac45e5344a0256facb3849805b8a89b89d3cf8dbea562637eeeb95c27848549e3f2f8055c8372b24f1341f4ce6483aef9a3e028a89ab66" },
                { "lt", "1ea3ddd526115e6fdc8b75fded9a9cfb38f297ec9748b3c4bfdce622b19a2e12251e3d5b93ee6ac25e4f24c41e3bd890cf6d80e76fc2192ee6daf3e977e152f1" },
                { "lv", "a612bfd29a693ebcc4e5f7d551b94819051031535444fcd491a52fba2625133d1c4fe6484b310e95bca0d6822c96b514405b5394565925746297d44317855fe2" },
                { "ms", "a8ecba6c97633f24c32a2346cc60e50991dd100f787df5ec47def64394c6518e8941f37bb421736ca39d1f9a427089d63b8fd8740a030712cfe694cabe44cf1a" },
                { "nb-NO", "f9135d9605951299cb0f23ab132a24389af1a513ac3f4e433012df3be68533757489922fbb67d9b47809ccf65a33023b79e1dd4e4677b9c6938d0c783f5a5045" },
                { "nl", "6702628fb6d31a536e2b6ae2d8dea8854be28eeac1da1f0ee34d2cd960b968430d04cb868f72d30e2978af63bb035a6b5da36f44e6fbbd2636657c5e79947a9c" },
                { "nn-NO", "51303da1def8bfc5bad343dc55bd8a6cd34c0338a9b54a6380cba773396ce7e9073ff451c27f386ae002fdfe0c42b5322c8405523e6f5211a9765a951e3643cf" },
                { "pa-IN", "d377dfb9ec93d1c16d4408ec9dc4a4870b20e85c52c14107536afda1eb7c3ceb79a2404c90a90ed783f9b0e2fece6ea32037a70a65fdf990945801cd5d51aca7" },
                { "pl", "86d36440c46cf81fa3034ddcb434c888d995aa04624bb0b3535094043ad5fb3b63088bbb948b71abc8981037114b0926623ac6c66c8925c32482523eaf790176" },
                { "pt-BR", "05591997c4f48a5e7dcabad93569bbd79da73dbf3e9ff759ef16392a6066909129db86f1ce003e2496e85d25b77974eccc47cde1d17eb744a5de8548319d530b" },
                { "pt-PT", "4f29eec32362a065bf2682752d6881439731c578116569f62e75479b50f1ba0ca8a0a29a3ec9b990f2af0af593cd5db1502fd2f6a82363fee0301b0d31b0a787" },
                { "rm", "012609931cfd64db19800328efe7cba87a326081853d1e44e3018e459f4496301f1a90c05674108e74af701ea9b7bc20d4705048751cebbd97ce55f9f1098b52" },
                { "ro", "eb7b930d53c7fccb3ff4fd8c707ff437fa48159e75d60d6e0e7b18ebd26d1f1d21399c9f8f30229d256c52b2432eb97e92ecd79edb68715df7aae37ad260181e" },
                { "ru", "c6f1b8f3710b422dcf1b6daf59417012ef823b47c4931faa83a04767b1e3cd1522f0bc5340e0107ebe1495e74667942cd2b24835c829d9422e716a5e288c01cb" },
                { "sk", "8a8404808e1cc7eeca95c49ca3d720253fc3bea3fd15aeb91943b10da3b625b54ba36dde6b5f056f3a26c67e7744d5b7c84fb75a99a149b8ff7a086e1a4c9ab6" },
                { "sl", "66f812e764b9a78d75258a1548dc1a9c9b6fc83e55ea1846918f449cf4ad68020cbf928f2e1d057d358ec14f760dfc313274c35a1832c6510218efb05f8d2cd4" },
                { "sq", "e9fbb73d4a89819c9e945e37a849d65c4e7f6aedcfb019845dc4e2d8d9a4c49123f214fb55db457de03157500fcc4a60893368815e1cf94062107ad8a252eb3f" },
                { "sr", "5d2ee85fa7124339e3449e2afb8310ad4c9381b51c72fa7787e920a9975c1c9cb25172087007bf4dea36b311429b784d39df0b303247d96b15dfa0a3b4710f59" },
                { "sv-SE", "e17a5f3561b604e7ba9dc6e2688dc1933f311897e44d9c573b1fdb65d6417c72d6eb632e5c7c41b0ef3e21a83600291e2dbc2abe05f7ece938eb68ef8db43edc" },
                { "th", "e342f232f738bb385feed0093e224935cdbd05af98a1adf1f37e27e181d19a1de2390e19879c3818efbb9448c6df8f81c042bdf9f59ec6cf5d383c6baef62284" },
                { "tr", "f0e2f4e13c898cfc6f7b8662e2e624c7cbc6c8ed9565257a750f6d37d41e22b8c71157fb13beeb467ab505bda3df4ed2ec62c80428c35b7c6146c8b2c0c18f76" },
                { "uk", "9754f844e08351c605b80e05b26cb8ebdfad27b019235a47fd707c5bd0209a150ef6cf31b02eeaf758eba50754ddcc056292bd775357ec27e6042f2c27d8c3ef" },
                { "uz", "788d9ac6067123ec2455310577cd8e48563742ec71050aac635ea4d29d15048afd467cbee4b6f1256eb83f8c2c4d95476e2d31c6aa94c59bc6ff571b43a143c9" },
                { "vi", "91467af387195a2d72c2b0619611caed535e41c4e84d2095cc3d1bef1c64074496d5921cae67256372a3f1f63de640cfcede029e27b93486076aa38288409a83" },
                { "zh-CN", "17f61ea3bbb207158bac67d32204f51dac675f72d7a5b2ad2431b3823bbc4a93fc904bb9d91b983da24e7cb35742512d3c1e87cd536af9eb4a1784666179a71a" },
                { "zh-TW", "ae193c9c6824a766e1c9020b11b7c9e10a8cf7bef1e8219721168441c103232ac7715c027ab1922eb5a9d875ffac8f7676d636b7436c4d5cc7ddf4d0c31df730" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.1.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new HttpClient())
            {
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
