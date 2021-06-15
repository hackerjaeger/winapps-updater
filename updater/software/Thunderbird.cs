﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.11.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "9a4cbbde1df487370355aee9876332f5eed864f50493f7512d266075addd34e4e2e17e0015f92559cb41f13b71010f8d2e6c399b43527221a706e0f2fa88e249" },
                { "ar", "9222d51b2d0c94dc1c9d66eec378317441f36567119383dcffc61819f6cfd855517e24e503e44196ead440557ebeb6b093b2b83050a5d8280140aa72430249c0" },
                { "ast", "6cfe4f54bff28b645989806b2c6d907beb88111a0b4d23913737f231ce6e5099b4c97c2717cc4068d7d0d51400a1cae762f7ff0bd1a9a8a235b8fb4a7dde0650" },
                { "be", "499a11dcd4bb32ea1821f523ed85e337a5b6ba51d73bd392eaaf69c205b208cdd917fdec90be7589096688eac53c0b067bf885c807c1543109081e8571927fa6" },
                { "bg", "13c89f08ff7b301d35a6fd5f13e086cabe63cbf9eee471d66e16e8b9e1e521ac84a7bd945896662538153e88d95f59d14dfe74628bed6a498fc58a34206c4e55" },
                { "br", "fefb2aff76b7f04d9b40df8088f969d93852c950441a4523ab964952a07686d55de4ce3632273800ea049632c74ff18bfb03c5bbf3495ec2a468635f6c47c20d" },
                { "ca", "2f8490c2638b477d419e51e3341802dbebce606e8809e02adec13130ecb719ebef836623111ab6b2c6f98786cd65d61937b665f9654a23629fd55a41f2544f2c" },
                { "cak", "4e23755627f58e9198678f9d9468af39ba8f92f89da42b088d17f0c38abff4e10e4e038c621ccf8e03ff86c611959d4ec898fc36690765e74c5be6bae7ee41bb" },
                { "cs", "d9bff0273ef78c6d98d4a1e7ec0817f26de6f5b8786dc5f95db73610544dc145ccfd5d9a2441c9b65514da9e5dcdf481b9b705ff80824198c3936055907dab6a" },
                { "cy", "df050dd61168df714c044b825580ed35f12e2ee78fa0f1ed9f791f8796d909455c1638ed791efdf08eee5afa5299a6e515fb1e67066f144d437dd1c8e3b7a0f0" },
                { "da", "1c033a18fb8a1452137731e1fdcd4f69cbba35244afce950f292a3320a8f33b328f98d6db7de8d671b79e3b048bcd639644b7ceedbeb9052d5dda6a14a7f798c" },
                { "de", "da82ccc779424156293607a21043cb3f8ed8b95c01e36cea36cbb06ca217c93faa15f3573f35cf96778585c5492ceef2e3e77435cb05ebe949a5718a359b3d93" },
                { "dsb", "285d0b72503f2d9789be2fd4e346cb3bcb0f578506eeab52e896963b4cf4db7104a6bb9ff90e12453ccfd4a1c4bb57605a09dada5d54f89dda8374b362865c4d" },
                { "el", "721b5828eb6c441470aa6b268a5d4900c2165a7d9426a9a391c9de347d1137e788a6d2a08ee60c890900032893ab1f9d6f22d0b80a2e627738cdd4e014b6c0f0" },
                { "en-CA", "ad2606eda9199663440ceadc5b26f35bd15b35b6a3926c70404e88996a275d4aaaf87e8e2c16b676972110b5acf3a49e865d28cbb54ca44311312106e4bf43b4" },
                { "en-GB", "5fc5389d84720696f61cff99fdae3d47c0239575a249197803fedfb2deea470533f5101d52cd13e422dbb4d994f4a2880e2e5818b0e5b74f750529752aaf60ab" },
                { "en-US", "667929ad0eba53df11a4cca9f654137ec99e9c16cfaf0ed255305d1f55e929234eb5254421a8aea601b4c8d17f0bc00bd62c6e20569797b53662ea1d47634089" },
                { "es-AR", "11e5a1e291e4a2201ef09dc156fafaaae95a95547f3f54a161ff835285a58fe4977917f0d94d06f71b0f9dc8efccd1d159af71cb017c3e019df732d020678102" },
                { "es-ES", "a86eceaee3830ece4b988d992bb5431aba4a97e10d14d25ee4fb3ef8cd368ba05475aa145391c5baddf43a37a74af77c0a59dcd1be01ed1743d91d232d96c10d" },
                { "et", "55762cfa2c3d8ede0e952a0a8d6f2fb7cd4473663a704548366dd364633298e98a4cadee96dbe6e8338a87e723190c267bf73f67961289138ed573343681931f" },
                { "eu", "196751ed5c3f437ac13dd65a6cfce83911b0cb22c8b15197fa3f3eeaad5113f453b578636e990d0ba5284fb9baac780a26211cb060a1d34d1e328cae47e8f66d" },
                { "fa", "dc4262cd99452ea8903540dc19096b417777aad996196da79fe807a1a80b464d9fa331edfa89e657047832c6536b2a09d4a2cb78fdd80bfe29790f7742013b21" },
                { "fi", "11437d33a71109a7091a688925e3d75bf486bd49e7c91fbbb09ac45d0ddfb91dd6e9dd4c108bdefd957e38bda360e24e2d253f69feeaebf9180e40035151801d" },
                { "fr", "faea52a50e5af75c5740a0a3fa8a58feb624cdf07f432268dc56a72b7571327a6ff2f820844cb1366a8236e61754744ea56d9f418c9322efbc13ca4fd61c39d2" },
                { "fy-NL", "14fc1574c4290b387dc7eecf97a0a5f53b39ffbfde75124f70d0330b869611a1cbacc654d2b4aea98dd9d6efe7ddc9a1ede61c2390fd9a5a9896b693d11ee48b" },
                { "ga-IE", "b575663b6967c4f463796580274848b8f2b183dc5e7db602be2418fcba412f09fb6478cda2e9f821516b7c1fff2fe7f09cd47d741d47180f371c6674b9ea3b6b" },
                { "gd", "a53f51a6da38dabdc1ff042ea41f095261ac1c169fed6ba1fef6e44cf63de745decaf3a5aa0537789dccda801267a0669166b631d4025e453f8210d3e90bb4b8" },
                { "gl", "953fcfd1ac259437aef894a8ef7fd0a5a5a46fea70d4ba2ca9a1ab1d3cc8f5a155a769eb605f19d42fc3e1359702b00bc48d68dc6ecc21faf3e2c19f0b1de238" },
                { "he", "ef8af2439714bf38a43b2d4a368cca70c115fa493fb990e618166ff4d88cc872a04fabf540e0c23b7e93b51ccfaa5831d21028fc9a7de9d461824f9741719c28" },
                { "hr", "376571e068bb4c1f1fe52d874064fef82c7919007e74a16f59bd30babf33b6dfdf08ff3ef095885d52f5bed8a9adb52cf7f3476a4441b10b4c52ecfbc96a9272" },
                { "hsb", "578f6489c280f85d85ff9aa219da551aa59b466a48f20397da854c2457aaf6ffc47099666671fb929c2efca5c30cdf9dc5d203ca5fab389caa4406fe9bec89b1" },
                { "hu", "85ad86a83c41e0272bb10163c9ccee00b66083335ba03790389cf55d5b276833d03b676f2012c6b1285f80af7a6b533da1ec03fddc0139a158a39ebf2008226e" },
                { "hy-AM", "188ca442c1fc43118b7a00248dfdb790f6a09c296b94817ce1a76bc133950291b0e3053ca66ae644fbdd07f932ae4001ed37872d1f6e355c8b690d512a8d16d2" },
                { "id", "155bfe731275ce5baa2e0cc50857865f2b4e874f6f36595a44cf82b21ed2155578a5eda7bc3427b3baaacc63df18a876a7416af8c5c898659da4068654e6892e" },
                { "is", "46bb3a6bb18ff926e254c236e722154ebe6fb9eea2e2f01446be1ec95abaab9488cc3142f11ca1eb6bb09c48bd255537b2e544252ee8999c47167a1925e35ade" },
                { "it", "52af1952462c8b879ee1ff6055fa0277b34445079c4f15df72e0676002215b9eb7025ecce4b81f89c296852b45bdfd48b76c59ddb507e3062470adf0901ce788" },
                { "ja", "173a8e5c53771a8d9881046a90ab9f6b55ad1aa11067703b99ad4cb20f16e7e074ff0f240b85cc62c05474b2bc7b79271f522c1dd65e5e17f939136a6f53284a" },
                { "ka", "0373b8dc0ca904a9fc449199d67989da8349280e98f298ca5f31d6233bc334fc9256d15620b7d4b564f16c1aceb2f088de495ff48c99e464e3d9f309972f4b4c" },
                { "kab", "b9fc35cf1335dc1e67b98a42c177893bc66d3e27d0052a0f9a155dd95149a83fae588ce3e685f64801cbc11efd2bf608a82bcdb26d406ff6b910a45d968b1439" },
                { "kk", "493a4a7448d24a27d5ad3805e30758ec221f3452012b96274be69e1c930f03418c4e0129032830b876a5a5f32ebc74cd94bb28eddc19fc0e4ae66ec1b604cbea" },
                { "ko", "c4c1bcc2dc99c553345906438c1607bcd088355a453dc133968b8ef52cd4f5ddacfe23e5a70a0061fa5a7f813af5c4d2f57ed6f99aac1f43d21d13f0312c48c9" },
                { "lt", "623a412d11731bb372bf08c232cc47e7b8b8a412233ca07eb5ccde6c5547712a0764c9c967f3a8f8c590a967cd9b76de3021069f51e20bc497e327fd912752dd" },
                { "ms", "ed7e412526cf94f42d9727df5657f3c7e00fe78421323547a27f1e561d6cbe151349886efe7be63c6548fe3389787e98514411f97c5ea716f0087de0e67eaf2b" },
                { "nb-NO", "9d1fca83638bde4b8b6018a873a7150ac5e86c4f0020adda940df4522b0c0de1d03e07cb61e2f6e865be3f2a9a9df15cf27b243f9ea5c5a74354c0901a23b3f9" },
                { "nl", "6816d540e445de62edf3af18629f73b46cf0121b7f608cdfa02ed464542826fb8af0632716dd5012990eee27c730d8b49fa0f835be0cad9c09607545fc36fc11" },
                { "nn-NO", "25a1cdcfe00c349b7c350a4f9608d2335e585d53137d022005ac2ca5f323a0fb157be6765d5d162c3204ea6c577b887e12de5935ac68121b9e6dc107eb1cab26" },
                { "pa-IN", "a8f32640572398c24c110b53b3e89c70956406b53254706090c2a5d05efca84776b5a0aa6daf8ef8bfe95995d2fbd72853acee8e84ea87e9e3892ce90219431d" },
                { "pl", "f9247fa8852466e6561a9a1ea7947ae4ec3ad906939398f7923fc473c8d11784a5dabd0b35e8504806f2cdd97f6497ebac7a10ae6af1acf5fed57e64b048b6e9" },
                { "pt-BR", "8a73dff38ad63d1e0c5a65122094668ba3be460aff253e82b6e9e073ff683e32d49178539bc143c3a16a8512901b07b11bc95626f33b77d33d658547f752b182" },
                { "pt-PT", "53cb74234ea445a0b20f296df5d803c8bf145684e6661c79aa1396135de074b74b35f85e217d601d6923c3e46ea2fb3d17399ef7aecd609980c2f72a30ba766d" },
                { "rm", "9a5c4f0b4cb91b98bc90e866bc80b9718aba75e6d1f9671ce5204cc541a7a5d50727ba9304e43f25f99bfe2f29a6e0c7e5d9e8ae117b64cfea0e07522c44b559" },
                { "ro", "2c382da0e6d9add3c6e6ed871b474741b90435063a3481a1a37d4c1b96c1e62d6298219789745ea7d67f80d76fd8efcaebd266695c6cf7ec06b199dca820b81c" },
                { "ru", "4edb03418036c36f5fa866c5e4a20a322ef56f164fd97a672e115cbedf46773e52e0150e24fd36e73e8b1bdc5096501565d389ff5f0f3ab58120c9e0c0af2eac" },
                { "si", "15ac9911e3f4a7ab558ef9a108a2d9f8938c9811dda1bc1254b8640f4f78a6e9e349b7fbdb0bcd9d44230866fb696ad0ce64328d0afc3ea925a57583c5f6b849" },
                { "sk", "03f3c0ec5bc1307d5a23d6070db409e25de7b29e3c56accc21912716fa3e1f9edf62a5f848712c2d3675408ab3d74bedceeaa1ff21442a886e6fd1bfeab3779d" },
                { "sl", "eb8eb01d0f5b77b590cfc24bbd4d7dda49e2a0567d016abb9b399346530588f0555a367bcda73f907034e99e3de6e20f91cb40d50890df8eb9f7c24fed04b042" },
                { "sq", "9f7d1b7133389fc303d6e90a93f1eaeed074b99219662da27e95a8d53bd8eb31e864a79d14fdfe2f15a0e14523b02998bd514e450a6932966911809dbfba76f0" },
                { "sr", "2aa2e985a3f313d3b032dbee96c8986ec7f7f273d14ec0ade79a4ef0de74024e58d8a4148963a220800c6168c13035ecc30f0d7cd6df6f074e63e717a4cb8819" },
                { "sv-SE", "9fd3473019eb10a59708fc4f2f55c31938265d9044ea8c857acbc289d5614ae717f33457228c5fe17b5946e10f100104a565ae04750ee297a41268a8f4e0719f" },
                { "th", "e4693b24cf7f5f92d7a46ee32c1598556b57dc3f5f3b07887bb96253def208a1c8a55b738f998268a135c959c24f78c67340f9ba621cb26737af58f3438b8300" },
                { "tr", "1f244eadb1b654e052816c3b1df14e36082250434239a0c69162121f3cc9eb02b03ebb5ebe12b5a3009efad888109509179c9c607b822eb9699093d91dee0d54" },
                { "uk", "805d0f1d675db10c61b8340bf7e37592e87da576553a7b199f6d9c0283a7b78f15c7c06118bff76192122014d93a00c94ede67d55b9245dc585efafffa60dc3a" },
                { "uz", "3dccf3edf1f310adb95b23d3e763f2690bee3b6e12e2608a2bb61c757f589b241bb418a60dd6117b0c9231a23a1f4f5399da388f493f3b7842728e5257472ae3" },
                { "vi", "958b5c4bf54da8540bb326f11d486c8108e6f69b416e259a87463ff48893f90252c075a3e08b1b8bb36c38378708b6774da0017ebd78e987825cb872f7e568cc" },
                { "zh-CN", "dee7f339e5dfa6ba3a922a4b4c306dc07ddc1a835ab4dd4ffbd02c17bbb1d723b71a07b7ab042188599d4bb775cdcf3495ccef2999349dd478dd4d61478d29aa" },
                { "zh-TW", "5bdb41ca385745e6548258455f7cea818149e540853dd319610532cc96bc96299bd0da75559b641b022841fc8bfb8a4536e8ec57b844014a772f7c6ac9218758" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/78.11.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "79690b63220252cb9c127742ebc682e7e37dc8cc00d152e86a5c15ad73f8703ab1979c4df21bab5fddbb969333d9d49d18574c8dd5874da77adb6b5da978d007" },
                { "ar", "f09aa1bfd2f6f8a0417f9fa5b455caef5bb4e31769ee71c83f3c6b5abaae304ce64aa0d038c9e9c85183f2833a3bdbff9d2e0ba173178cf7e83bb9816c5ba4b3" },
                { "ast", "d89eccba2d209295a7e54dc1c353acd527017cb7582c4c57100ac02a60ef2a0837c5dc7e4abe7c8d740eeb94c28979c1362f9e7706ba040c57edeed4e1443d27" },
                { "be", "f346da74a610c210171527dab4daee9543cbfcf817cba0e904178e507b8542cfdabb55c3604d8dec1270815f66c2dfa90a2efb39cf005cdedff1d545a4e38ceb" },
                { "bg", "917aa37ecbde1e1e1864c89307810c4a22b1845be0680822c62fb8df434a9163c7fee3f9874946f3f1994ab2c63415000df75075a8257fb2d6388b549c149328" },
                { "br", "5b2d689bc384bd8b1ca510320ee9e67568fe83d6fe8343f2c3827077ae9cfb458ea2316919347adc40d65dfa0ab76fad452aed9a2e8c5c7d1afc8e5fd6390e2e" },
                { "ca", "e0605d970da12d5258f6823d554e2ecde716eb99774fba73f3fe990186cb4aa5bab84e9a6516c43e2fdab3befb85c39f2743c6fee19c3f3a8db1540887a75d14" },
                { "cak", "60e81b9216a471c3295b4995d0dfe319bb02e08db2a1640eb9cbd44f7f21f5012565dbf1e300612a5ce27a696a1b8ecc202f8ae64285ed5c0451dedf089bb2fc" },
                { "cs", "b5d7913b1a6ced447212af74ef789cc9b43a8a2fb232dc9a2ff4bb9755a5d3cec8c9df69ac4ff8523dbf2d3761036630db50361623007dafd58efe3e157fd03b" },
                { "cy", "e429a69140f4d71e2620c6c2474ce807633ca45a90e043581fb46af4bd58782ca9ba49009ceaffcfd7a7fbdb370aa286e2d3e4d7b9a5a5adf902f95d9c64c6c2" },
                { "da", "95ec897157b21d8919e03bed7a2f0d8c3a940b0ccdab4ee584406c8655b60bd1496c8329badb85578d6fd0b71aed14ba5db7d793d298519b2e3acecbe117edd0" },
                { "de", "9daed3ba3c2e566ba0e81bd77a6b8cbe4536a6adf87190c464a03ec7c6d1b787f6f9dc4cdc5a1dd6649c999ea901a5067032b5d9311de50d2af1702b93104888" },
                { "dsb", "efa82a751648643a15b116e26cfe5aaf865b01daad02710aa0f4111b0d8a4cce8766ae78a4901bb5e846c8d398f83f5df1ab4e5af62db95aa5f6bfe65200fefa" },
                { "el", "8aced20c81a2df384ad69f06212754a9919fcfb95cc64e024b256da6bbb4b03bd0c9f36abcb0e6243aee7df585ddb7aed1cd1da0bb31adde9cc36b62209ccbb0" },
                { "en-CA", "f8a35275de70f44ef4360dd29ec2a3db820926896027db77f2adb2c613b14f4f76f1009c57258565c00184ce840c43ab2bb78a042bfa0a070d52469abc3db1a0" },
                { "en-GB", "89e8e5090f0adce89a2ed1f2d8a726deddc2df57741ef53b9282ef7710dfd5e87f7dcdf892aa37647e72e614689e159d9577cd10bbb95fc7f37e986ee067373a" },
                { "en-US", "8ba5f0378b7b2fca812e5fbf8be771c61b33bd716eac95674bf2da886373ffbb1c602e40cdcc14fb23cc358389f6ff0583b8c1e21b3a01ceb37ba18a047bf686" },
                { "es-AR", "2c64473d6bc9ba59587dfba5de44221eb58718b18979f94fc31eb3a9c3b35923184ba7b02134c70fb0e2aa970efac1bd5ee6ede742e48c757069a084e4192edc" },
                { "es-ES", "3303e73241073e917a790a745606cafeae59ffe70e64d2d4864ad787d49445bb81a5b4ea791bf576db1dbc462133a08ad9f32fd942fc0a074e8c5c5b8df183cd" },
                { "et", "4e2fdae6bf66e9b7565c532f1b5b25a940d358e30398c899b66ca94cc89c069e7dce74981b013c117532eb7b5a1279f9d20b5be646f6ccc3204f56ed11db5e5c" },
                { "eu", "542a1ef59f8b95621d3fc097b801d1e6e3ce97b971f67c2284a6045e964fa6b297f6bb10cbb81aa2f295eb125e965336dd99ea7a0373423f7beca3f71da78aee" },
                { "fa", "d6ae05b85e846df8d6648db44e9ed8630c79988abe59ecef3d712ce9d01e9d589748cf1c002eef9ae7b48195ea3a05325678e6a8b9288f7fc43552b27286c1a6" },
                { "fi", "9ede03d07ff5bf12a62f14bcc43c13498b845376cc3dc70f99c6d2ec8a85cb4af38ff910531504a6c3664fe94d9876f7da9db268f3977f173a109f7e5341fa63" },
                { "fr", "9a1be31b6174ca20c99511e98019fe9df2794c2ef80da8662f4db42a71e19d62838174a3bff5f63e0fa0303356e8c7856dcae2d181dd09fc3f1a36fa4c549f1b" },
                { "fy-NL", "47cef79462974550d240a6b75cc1d3d4e3c3368d170076d81d369a3caace270ef2d3dc97a130694290cf004bd8e5dff7c61919f46b0cdd67884a884e8b65a4b6" },
                { "ga-IE", "a6d9f503cad937edfcf945dfe07670ce541bdf25acd5c0c227578c87e88809c8c95f8df426e243107756e0834579a43e81b5035dcc5730801d84cbb0acb68d15" },
                { "gd", "051132d078fa120fb1de5df540a0218acf80e02dbe0520a9ea6b11131ffcf8eecc48e4845b7c9b3120eeb4b18e10aab66b1fa282d7ec342d9a45e8f956eef526" },
                { "gl", "e245dad75542697ed21b865e0d11d34e19a658ec63ca1332785121147f2e7984943402464cafccd29f441f052fe66aaca0deb5820139a1d6dd36f4a1c4a5911d" },
                { "he", "c591d05d5f56a7560290797ebf591411cfc69f7a12195dab806eecd3f1cc88e6b98207686c992115410fd5a711333aa6011c0de00c6157ce59df0a67c1596a40" },
                { "hr", "9c8b14699a12a59c4c5c5b2ceb53ac1f16ddc93d2c89ab03c84306fef9efcd856a2011727d2c9c434d8f7aeba202f6742cc5968350df38e0f65ff94529c40af6" },
                { "hsb", "e8746b4cd987a62a2974e2326dfce679c68b732ead5a6cdcae105c8ba17ef6f1999851648e0f53275d3ef77ce6ffa157a79d8cc6e77b31ef6c5690159eccf8d5" },
                { "hu", "3fd989ebcc400ac3de0a4a505c31d4c0aeb7971c305213084c426789a7ed060dca88b2a2751d0d8e2cc81793641f07cc6ada66829dbd73979bf2d1b0f679b2b9" },
                { "hy-AM", "f84990839bdc4a9b580224e89aecd562ea2f5f04ef5b82e7950cb66dff3c156e87e8f2e1200559aeb458f5c1aa6c866e6dd4245d7aff124619655fc0c360a90d" },
                { "id", "9be302fcd3027aa2aa576ffc6a75d26d858f342493fc47ac37844c27a764afa5c81922741bde8618a1176711510e59fbaac430bb723a2aaad6d6bc59d7887f9c" },
                { "is", "495cd6f585a01d9b6859ae6c3a4cce923335f6a1c8c19e8b1bb7f0b1bf152fd617cc537079134524c409121ab3c24ac25e332eb6a92d3ff395aff93178590dda" },
                { "it", "60147bc505d9ea30c89762fb0cf4b460bc77084974c51494212cf6b47d1a91c9ff0ed7c4ec914ce361b6201544960f49d1e8c8d4794bd8cd34755a50d200f66d" },
                { "ja", "dfcf32f5bd7e65ad5a270d9dfe1037abd4ff7f8c6c82309538beaadf6cf6c9d8089f106889faecf1a8696da91b106d874d55f3fe65e65c3fdedf2c08d665ff09" },
                { "ka", "6abbd7304d9e35f4c7b891d14399ebaf930371048c39dba031c303d87537faad72ade448230154e1dee90ec9116f55f5774abac38ba3b8a1d14cd313cd6a4279" },
                { "kab", "4be56316a363109f5b8b54de955594289aaecb64a25aa122a88dc93d15d224ac6e86552ee28a4ce881f6598fbed242d24c59e8153167b9e24e01ea2ff1eea141" },
                { "kk", "4eb1eb69dfbfd6c0e33592e7fafb86b93ce68ba6055f5eae37e161106c81dea3a720a38089fa8990e48be0b03c56506cdfc3e4c8f4c069fe7eac0ceb75cc11d1" },
                { "ko", "9187f31c18a3d040b14f2147f1e14d1ac12378027152d91fa73b67bfb1b2703beb1f772e5b9a81fb70282d3036f2533c9a2c893d58411a563fea88bc7e980722" },
                { "lt", "66ac3aa77160f0d59e63a84180958f6f0cd254014e4c8e49aa09385c9708ee83d5f8f3cf9ada8771e0fa4618127576b40bf56a867c24a2828e892bd3676aee09" },
                { "ms", "dd9be30f3dafe06458934f9c06b4c44e365cefc8237e3343d9e49a69f4c4111f3146255ffe031f533ba101f376491fb6ee30c602d96654ce0101b59a1290a9d0" },
                { "nb-NO", "9b53ef45017b6d5cc8468075e046c248d3cfd2d4922aad94322533e3ebccc40cf80ab76c4bbff4c29c51a447153e66574f27952542e79e1ae174880678e5a000" },
                { "nl", "99c663dec0bf0d7889c76db10f8f3e689fe2df0badaea396cfe3cd3e484269df444dfa94208728895b0417742d61661dd279275a23bd78458197e1f123d05abd" },
                { "nn-NO", "cbc2863d02d4214c931f2bc8cd7a8ba1e434da035450a3f6dd144e43600b68660e8d41b0a48bc120728df344119cbb701513b4021fa0d8053ce4bac3b1fafe2f" },
                { "pa-IN", "1f29ca9c999222b38e43373a3ec4a6580c86343e6d3d32de9f77fa4dc05951e114ced467177a5ca14d14b86a80ffbc7dce46f91381a490921ae947a4c090fca5" },
                { "pl", "1ca021de0b4919f0e3034ebd8e3b16f48a5909ad1b03abca0c80500c6845c7c23aae00fde7e17178873ae6e3c5a0498dbfb8c876e9708c509bac0c429f01baa3" },
                { "pt-BR", "08644686f9dceb999a79d021e2d8d7ca860e094f5da65ca25bc3335cbfc2e7fb66a2510810d44933f06bdce7a4eaa86ffcefd550885d316d1b007df714854aee" },
                { "pt-PT", "c9188c81c6a849300120519679154dc00c1c20edc07d5a3effa2f889445c15db16467998eb8153ef9ce4f229a63752d4ccbb0bbc052542c2f55024be0a112a9e" },
                { "rm", "48db7571ab5597f7e3d11ccb524979835d9834b5ce9ccf2c1a62ddb5f12faf0d6e4ab398b84e445b232f6009a7106b69e06cf19409a82e383dc1e80df9a3b731" },
                { "ro", "b9ae76c188d346661de0f5982e83abbbbf53592fc3d26098369d6ddbd3fd170cd7a12c3cf9bffb742fc669022b40501adb6c66f1a07ea397155b11b9661cee9a" },
                { "ru", "0255af2a1513764ce53a175b5fc13096f6d09f48fc3543f66938306a3dc6cc5a9f268b5eb6fff725cc11ec7a591ca068e4f8c6e548ae769a9ac68bd1bc34ddbd" },
                { "si", "c1aa9b123f86c47529366fed1d0fe63a737ac95be964fb3f1caba530284f2c2d6e690854c009752f3debe25e6c2584f33817a15248d78ddcf76c9df1cff72c7d" },
                { "sk", "5620b7a984a232a92559de5f45cfa4afe7a74581de90bb4af3014dc05207a521ec03e114c1705fb7d50b201a3c2b4ed247f30a5621ef9acbad5cb56f8ec842dc" },
                { "sl", "71bbf4e500afda32ce62caaf29ba03ed03652930119778f88f0b921d4f7dbe9d70608f39c83f4575d2feeb37cb41734ae8a3e63070e799f86dd9ea04118c92d3" },
                { "sq", "c337e0479005c636b67cd667c12cd51d34e85caa8210b7585f8ecf55e0834dd9539d1d6b533fd50f16322ceb24e094a588ccaa825fd6157fd5b3d37a69cbf946" },
                { "sr", "a9ea27db378096fb7108794b02123639d3352b1b560ff6abd7651d0ed38c74b93d452ecf300adb3126d4ad503193fff2e44d5858e8a624e3d646217c8665b337" },
                { "sv-SE", "b9dd48a6d74f80107234909e7c6f998a1f490ee9bbf6166be53c8e66113d9dbf6a5e88bc2ddabc8203ac705ee0e2558d49bb5bbd565fddcef7c8c6edd9054f03" },
                { "th", "bfe9ca047473ea955c5ed34fe2f9ba39c3a34967ca757b2da3d9b90cf21b72ae04a567ddc5695f72659a901914adfaf85326167bcf691e8bb4e800ed2ad421e4" },
                { "tr", "2f18bc74ffe004ff7ca449d3ec40ee0076d3c82b2470ac65042c2bea8e7d22bbb27579488f328c647b7f3365dc65f02d7d5d59ce6288cfd4b85d81910aed35b3" },
                { "uk", "0c87636c335364a34827e7a488abfbc4ea0498af5a80274365255cc020017dbfe7ce0dd936cbfbb5d23ef559cf6742c573540ec6753be929b3e2c2050e7e223c" },
                { "uz", "7fb0f6ff830666559f59ccc5df356525c21279f9c439d1d9f9606dcb760d66273c74450719fec1f88d55aaa1d9373ccc8cb384d3842e05729fcd439b171e68ce" },
                { "vi", "7bea8d873ca97fff03471749edb24d8131d1acd6cc08998f2bcbeaa70f83a5acf544ca34ebf02c956e27180de83aa06c7be620d8acb7728c54e73a2b389fb7dc" },
                { "zh-CN", "1e29bc749b45b111578e4931ed41173d41dc15d8781a88ac8fa7b99a5d91b0cf87971c2e4cbd25a95be57635d2545c8120893ed648a4652901ee0b0f2b131f89" },
                { "zh-TW", "a779d5455ab65ebb84ca95a2d213eae4e0eddde2c233530d30302bd29cef7986d202de39af1736fcfa75d9047c1b250eed898ec38bd7d85540dd05c15e44cdcd" }
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
            const string version = "78.11.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird [0-9]+\\.[0-9]+(\\.[0-9]+)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
        /// <returns>Returns a string containing the checksum, if successfull.
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
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
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
