﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/114.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "3c55aa1a11d4c829be4ecd38586cc808dd0024278573d0d3d20d6ebc8d73de990988988f83aabd572d16f013c0ce16170401a6eb13d868e8a0132d0fdb8ddd90" },
                { "af", "7c0beb484589a621513b364b7eea84fbbae9a9539173a21d96d9b6961a3293549426915938ac228e6b803179460b16247809117c8fd9aef1a7d36e8f5f0d4876" },
                { "an", "9e976d78ccc7907273b8eb30506afe6985883ca8db6b3e6a7ca6121fe0d2223c75c2fc62b08e24304ca2d49ee692d3667c9276948d84f91606b26023f04e724b" },
                { "ar", "e7694aa30f0f53750e33313cb718f638144c04ad31a178964665d5de83c716a206d5599e9a4517ee33d1e746c7f3c1e9d0d98d950bac372a107778f54c2cbe16" },
                { "ast", "02df95fdbb96cdcda2cb688e9c37e76af35c1edb13f8d5540528e23492502945a1f120bd21c5a169459fb4d784ba0fabebe03df6ec7967f45a24f60a609e83be" },
                { "az", "c1777c7a46f32b762bb76d9e858198231c7be91c0fc07161a3b750ae4b2507648b8cf293f56ef73edaafe22fda649bcb18631d2cc516512e463e661c9cef980c" },
                { "be", "c7f0b2b88ff8dda69c7205c2d3201400781a595e346b3b3f317c5f8b5ad214866ce080d7beaa80b2e3b6f0fd4c1e5c294652dc3efc5821645c52d5cfe77fc6aa" },
                { "bg", "45386d0c8fa2f6484f5c13c95899b6e6f5deb636ac51b01567f979f4c2c5af36d65f86c49f422436bb6c0721df4f477cef801f270914e4c3d44fe19077d76591" },
                { "bn", "da5ac13c3ff81ab0fb31a5646b5495f6b2bdf8bde8ebb05c4bd59d849b2865774c88c0a3c4d0b8f6daf138a991bf56ee5e62b16a6eb5c65a91816d26dc5fe4f2" },
                { "br", "a1dc217afd8719f16c0b4cd8b077e6517e6d4255a7650fc3628006d60b8a27c83a4874fe79324bbcad404b1796578650c3a5f09962a58181457eed8417e2db4a" },
                { "bs", "e932f5f200509a23fb7b2ab968efcaf8d01000cf09124c700ae4b2d9aea331183d869a0541915468f372e702c25a8573865bbc25901b6764b344ce8461ab8ce5" },
                { "ca", "c816b5755c44d7d9f43aeed369acf99b1300750735118445d32edda6d804cb258c2d850dca825c3d37bd1c08c6f040f6b0e421fef5b115fbc50249cccd877a0d" },
                { "cak", "a04307e53695e4659a8e9bf12f48a3ed0c1f01e4f238ee45cee305fe9dfea3742b74ee1be174338ca37e36cf43a5c38571553493e691445c0314693b6f3f9f34" },
                { "cs", "b550a994cfbf7bbf66bf7cfd5e78556751941fa00ba6e47b590e83280734cf0625c2bf276ed50b561be001b92426a15d765a06399116c1f0e294dcf5b94b2f73" },
                { "cy", "3d4117a99c656ef4c3ab5fb74912913ce48028f69b5fbfcc0cb62ec812069e6e935bc41b68b9a8fbc09754d5553d5d4f52367631ee4dca66aef8adfcf16c3c5c" },
                { "da", "00e618121c4980dcd51b98826a7ace5dd903919caebe8b82da1a3a8d908f6989e71f24eb6ce1ee166f30b16077d239ae1d16430c71d2cf0cb6fd54710e86a1fd" },
                { "de", "cd186e04a6b1a4dff4f60bcc2ab89536a5ed6fa86e073428d47050386e8936c31ada770b7ff5de6d4c229b97cb66cfa0450abe0a298ecf2e932374033ccba9d0" },
                { "dsb", "221e1ad0155afcabb892e5b9897df142e7f5a2cab0248a4e0fafffc57583521fb8f75857661d86011676c605ab909e7d11091517eebd850b8dab9aeca6505115" },
                { "el", "c0a39cafb1b03d879bc356b8d0073983f139dcdf50a6ebd2b9a4b69eecb1cd659442b773ce08a039ded6fba5e5af2e75783c11a787674dcb3556a206e99b2a19" },
                { "en-CA", "62c673a1e0d2502d9bba561fecb4c430ffb5c80ff03fc992c14ea19f580b1feff9d0ba34fa13f6f2d3a6be1e34f42e3fbda706027381ee268dc48d86042ab9ae" },
                { "en-GB", "f4fbc5b5f579594754a25e8b96f5ac806d3f1c149e73e546f1c39a399354513c9e35e18f81e66ed00725df9e4b1df66bf615e950d411378690c6e18f7fe81a39" },
                { "en-US", "c8704aa810b6d8cc491096d8b62bb999ddc8621faeb5f071920fdd651f4f7eb9a4a7c6a7e9004f2709e24e65598fe0a4e343db3013a98039fe584a294772ad29" },
                { "eo", "62d191334f6e232376fa5e4c4361cf06a37e702a4fc9f99d516543d95bec88aedf1f34419bda740263e54fddef4f521273316a3264d8c59c50fcef009ed7d201" },
                { "es-AR", "c2f29e74ad8327fbb00ef41ccc65d6a1fa1c5dce5df8273a9673c87aa35db520f0cb9ced972af2f025b2f211e05e7443767fbeb80e016f1f43553bb1edc5b6c5" },
                { "es-CL", "9e60e27933767512db80b337ec1f70c0ad48009739c559c7c03c2bd74fc2c0c0f9296fce82b4237ac5125efed30085f2b651b28885641815a48295fb4396deba" },
                { "es-ES", "b2aa8d152831ef17e57efe048aa5a6f9e4d8c524c068724d92b9709dbf78234e7023f0b4e0c860a67320f5ec566e8545f63ede7e5daa1dc9a3fe82fa019347d4" },
                { "es-MX", "96a6a640a9f4000afe9c329ea55766314cb6c78e25d722dc7835499f66b87de65899776bc59d73759278c379e8b2e12b9656ca6423e2e9f37ec1abe7e41e0c5e" },
                { "et", "0b99ef1f1220592aba92ad33d17c25324b4a2bc47824ef4fc28957aa3f5f2c97021b71369566443bc115f3ca4ed72b5f89083098de4702b8b7919f571e1d4e0b" },
                { "eu", "704e2112d9c4719d80caff3674688de29433b319405e70f218043a30d7f62bf96425fe53b667257413ab931a044e167acefb47845d583a1367982abeb03e4e96" },
                { "fa", "edd831f23e697a72032b754f66d31b4e133106e5d378420661be2c743f2ba8b1f48f58fdc44ad76c21f68c1429f7d27daf58166f3b501b57a0b13349186ae024" },
                { "ff", "ed31ca5758a7c9d5070f61951454e535e068bdd6c08f9eb1a370d697d0dea749814bc2f8c3b00b293495e1afdf87235a942569892264547791b2776125c6f3ca" },
                { "fi", "1772ffbc5c81cb7eb0a0f18534a20c09c472edd6557dae1849d6fe9d5bf9db862f1d68f0b1fe7e41005f0bcbcc54ac3097f62c3bf42237b16a383e6259653a11" },
                { "fr", "b1502d16354d5fdd936ceb10309816d7f84da31659bfc411f2fed56df6130a5a0babb540219dd58b8f9750668bf1ec69924d3ca57475e77937727ed452afb366" },
                { "fur", "b9c81da7e4cbec015113ec4d811d0050c1b3b8675a373753785216f45e5423f8989938bebd2fa4e218a5eadafc9609879baa72dda5bca3189f413013e832fde3" },
                { "fy-NL", "b5dc2119019e2a91b9b3794fddd808fab09c28c75aa6b9110e1001db1e2bc2c647c8e2c427dc6856fcf58d5101702169d2bcc50813cae8f2917b1169baf2ae9c" },
                { "ga-IE", "7302c623d6d3a5780c8846a1a7766d3ce6ee7b0509886a22c8c6bf4c8528cc3a730fa9f6f5b2629015f8ab3b72a29a19f1d7d793092be230cf0e22354bf7804e" },
                { "gd", "6f25367d25b791c35a998ca24eb48b4c919027a32ae0d2e933d3d04a3182b29c850254f819e30fa2dd4507886d2942b342e8373a07344b3164970deebc804b2a" },
                { "gl", "077f68d0056df501511c5e4b7443409a9205c03a301ffe3efd9f4f47568a816f936c0f793afb3b1becc46bd37ca2518efa9c1bb0b6a8f91d7167beebdea05ebd" },
                { "gn", "c44d7849eced04b02c1c1b0559f0764c23dbc368640e29d178e7ab9e11d559611c89a0bbba92b10cd755e3d863288d48818e19cf067e02b0d281201a78ce5e08" },
                { "gu-IN", "5dc22f596b7b2c87cec7e0e775cc88fcc29d0debc55908ba11d53279b24e7dec26857712e2a6b8a4cd4ed59731374b16323066d2bc0c5b45460be58f2cac68b1" },
                { "he", "882924e1b3377d7a852fafe8d69b1759c7b9fd4eb76d4298ca3d896d812be4d679a03b874db55e62e0470e2d3b8e8f35260ade818aa4f3fafc8b1c25b5d19f0f" },
                { "hi-IN", "31f6e9207ebfc90eaf4fde44d540e3c7a9bd8fac3946f9e5738d527bb36452aad1518e7e4681b5f0eacf3a25192ce1d9ea455f180ac45b71896caeabdd3e567c" },
                { "hr", "707135ebf95f186642fb1d783b8716e61d37b75aaaa3c8034b1b619d671126565768b7494e09fd591b0d75f5a93aedf92748d837f27f2f6282ced52668081637" },
                { "hsb", "f86b58df3775561a031ba3a2aa104dfd8d49b7e7e9209b9ee5a5dd8122339defa896ecf7c12bf157f1390ddb0503e54929c79798cb46fc41665d109f118b4645" },
                { "hu", "8a73c7da7f2a5e11dedca101500eb1734e8e992f4a9190d96b4d6b9885fd37bff4a06a809f05804839fb9114f4ae8774b6c670aa6afc992b5a869db567add601" },
                { "hy-AM", "59c2aaa197690b20f2952b9b6cbb88eaffd8bf09d4f7a07f060187eae56e2d8033ec8b465118c78d5048c1a5e4cd6ccfbdc669cd29158a0d62a56cb5834b8bab" },
                { "ia", "8f8bd14ac3db669e1ee77ccd78eb063bfc236f9e55b2760dc368bfd8c2b5988775adf36a20bc4ddc4ad83228663acc387aeb62f892a5f4e2cb185ec050e97db2" },
                { "id", "889d383f426166da030d64a26dc206697b2dd444a7899f8638c2d0f94928560547665b1d0717e22879e4321ca73736c0c0f002ac234f02e6b30c39503502f161" },
                { "is", "1afe6bea82d50825d988d3f70a2920815e560de3ec18f259172995ed082548a79a46f3fe35682554e363c7b10bca7dcb2e270ca1530a767978a74c5d71fc0fb4" },
                { "it", "bb7e3dc555d6639cd294b3c779b78fdcf1b69bf53471ef1dd343c4cb7be03d66b6c35add761ad2b81ecc71039e93164b44f1299d4232718e5cbcbd570b776c1a" },
                { "ja", "eafcb76f67dee4f94d19e5f0640d95746a07be1d4fc6c706bf5e669339178bb04ee8434ec4ce1a18782c97cdf04a42021f7fcd8b34553dc50b063f13c0f38509" },
                { "ka", "0acd74dd025e3186951e77d2ef16afc1861e6f984a7b5e6ea0259e26894c160b19a6175844a883e6916c3952b826da3c66333267dfdca1e81cfb674e81d6c06c" },
                { "kab", "a9f145a9a113ec2432ce91e17a1c85296916d33b63f249373321b683f01dd49a7c90cf4d51591adfd4bd3ad288be815cdf58fc1bc82662b2ed20b067afca858c" },
                { "kk", "ba9e6f426c4dbd89a85db03a27bd3d1386b131317918e46ee7781f9e87208a4b60cf296518e780b784d77e305191663f9bb4adb26c8e0691c1265dae161ce989" },
                { "km", "0668b77c7d66e4b39b47aedbf34776a1b2825c3a50087130f41cc98036bd9bd6b2458f7cad7e7e146f9c335305360e3efd870e01c642ed2d35f393e554192fe8" },
                { "kn", "278a110d0fd7e0866b7f3531f731f174ea9db5ee986ca4826597026e1f89ea4200d09c93aceab2c32c316cce0b073754fa60c836be4c6443383bee8a596fbc84" },
                { "ko", "8efae71b882244bcb5e02b6430933c4294ef3f91fe5f9d20fd74a18116eadb0eae9ecb8ce996ecba770cf329215c5188c4e1fa8aeb240aeade6fb891d316e438" },
                { "lij", "787ab6885d280f768aec0911fb801f7023dd794966e3947f8296ef97001dd5a30a07bd0bba328524aec3b5844a7441c7dd3d1152656e11a0c3ab85e06a429b91" },
                { "lt", "1dff0bbf8b0b2ec2845b9db7b3e74f7ee94557da23a40158b41d35135f4ccc1ac1ee9c1aeb426034beee4947b7a7b9f8c02778685abbb52e91288aaa5b2cd1bc" },
                { "lv", "5921fb9ef11366b9c057c5326eb4aaa8db2197458b2dd1e64283ff988fa78abeaa738e7fd513a338d41eb19c899d16b195642645f93ff14cdf7ce75e2a177fcc" },
                { "mk", "fca50dc23d5c7b004692077b9df454a7f7e1381a5c904778761d439ce4dac55662e311b657503ec9225ec5084e8abdc20a2f702474871af5a1217883a63d126b" },
                { "mr", "3de05456410c63a2ca2c892c653a0b789d56c1c320e33b0b8c76414873a99d7cbd18574af71fdbeceac8f0d7d08a9bf786b0c023667cbb2e5a12b8f42c6c2973" },
                { "ms", "f6b1cc753e029dac2aa216ffc276d1bbd0ac0cabd89a451ca2b5bae11ae9dc3025bcddf0de765a6feb1e4b3e6527b9b0943cb3a35a27480db8e952f087456996" },
                { "my", "84b973635ee8aa02b2da03388e2366ff0a8a80137f5e503b1a37fa2d6d66e870a4ff0c2eb348cda54f6f7163f3eeaa61be56d7005bbf24aba0d2f4c39ba604b8" },
                { "nb-NO", "200fdf08ee7c372982637462f512e8ae3ae5a226204717a7bcf6c27e8285e9868508657eae21e1f5651677f5d96017990c4f958ca1eb8e982ddf3aa7754c0419" },
                { "ne-NP", "eb9c217838206dbe559df12d1f5f6c0c9202fbc69aef65d8ee1f92800f7630edbd576ba0f5864a5a7ea68240b43dcceba57b63f87b63ae08749ccfe8a8bd9604" },
                { "nl", "5013338d16270326f91804ead8727de81bd1a9939ae35adeeaaba4df0693f6aeacee97c2414d601c6c0ce90aea7aa974f10c0294cd71cbe6052a2dbe2e18d9dd" },
                { "nn-NO", "1a244b63ad0afadd779c3de513511d9bc2008ba22ee97973f1e7c8e4f1f086458250d4020522dcb78c38675b6724e72043ef3e6fbb19b3ca2651af836ee13fff" },
                { "oc", "9c0fa04faaff02787f65070989a29c5ab256aa1a34a5fdfeca9d71097eb8c11527daebfcea73c3b70ea67c048dc7be0fb60f1ea1b9fa25266c34c6bbdb9f4200" },
                { "pa-IN", "ba4374f179119e3afcf5075a68e34a0332a4d112a7aa4bce51d5842279a94ac8d5666b1173e53ec6afef18caf096252a84bad1aafcd1755936409f9080103a6c" },
                { "pl", "4b5cae19e2ee2b36bc2bac2d266c5438a5754b5c6486f5eb18941d0cf5d228fb7c68edaaca277dbb7004ffeeffb97de9d509c7820a319f9289970939c65dcbb7" },
                { "pt-BR", "0ec5ce03d743453c446e9ef657d552b03c2071cc4e88ccd280ac04b94089fa7359299ae3bbaeb1b1958c26751f0c3f52fa444632a3bafa77384ccd1a03f39256" },
                { "pt-PT", "91129c18ce5779e81a511e6e8432a38592d7beee6428912769fb0b8241010afd65525249445eb7bae01e3418aa1090b32d0f5392e904a7a6c0459cc9ddc8a4bc" },
                { "rm", "7fce2f448928b33409a17d35a7c5454eb9d58ebec08779985b344d0ea2a39314e665e7fb3f20a19e09e0d86287df2d3ae7b9c5b8f307aff3e62ce07566f05e22" },
                { "ro", "7fc8d106e01644fc3f54dc9dcaa3c66f0af89d77bbc9e5fd4e93a683c7b1f915895be9a9d86cebe93c0e9ff870ec39d8f49c5e312a54c49c86e48821d3808515" },
                { "ru", "6182120bc64a3e174536f91aa9ec15b662a898231b96002a12361f3f85fdf8ed171d7d676ecaa86f6d0cf56ae1863d6b85b8a8a88df6cc3ca521e6089c6d27cc" },
                { "sc", "d0acb58c499dc33258811d44050f2a0c8ac414649241f0ace31f622cca3ab4960a40f31d114c018bd706a4f1748ce3bb0b1d78fecc15b8c7500c2f19d2d08ecd" },
                { "sco", "e2fdbc3c0aec8bb14e53404eed193bea9294cb3b688aa1d8cc501222d5da91b07126fca96eaaa89af87036e57dbda11ed10dc74909544c6d5ec8dd26f93490a2" },
                { "si", "57df4bfd32c6079a3194ac07bd8d56628fbeb7ecbc03f939e7270627eb01fe1bff4598575e2e74cfd5d63bcf2cad2ec56ab0fc6226db9cb89319ad301a9b7efe" },
                { "sk", "beea16d893433141c073c108d69781879a7289c2ff58dd6cb812cab368f946e28f0b16994878ccebad1639cb2545d598cc71c335341ed8f05ba39981a3f7f118" },
                { "sl", "6748d08a38ebd363a592a7ae65c9fda11d88102fa0c452f0f2e203313fe3b8ef5c616a450d573026d0e95786351aa16b706ef666c375c25fd33331d190377e58" },
                { "son", "74cb069149d71f67fdf729e943f1cd141cee2e6617cc5d1df8ea56e31e5eccea95c34455da05be40b75dbd07e3b0493a8c32688dbaaccf8ed1e006f107b5400a" },
                { "sq", "cd769a84f11ab145757d08ba3e4e8846328433f889c63a04d5023b5d4f0468f3cd4f313481c1cc8113def511d7537a2344ff69a22745baa4ccf041178807c431" },
                { "sr", "f7fa5dd4acd75feaffb39968c79fc5ac28fc4bfb54923c25b15dc6b3a0724a1b34fde8e5c1a89bd2594cf2f291739646f752f751a4798acc7f94c34871142579" },
                { "sv-SE", "ab83f13c0cdbf9fcbe56822ba32b1773e1a56c9fb7d1a00ce9f90340c7744cbb038ace5f0e3cc9e92e84dc1ed36a7e1207ab23677b23a0d3de11a78db82e1d44" },
                { "szl", "287903e9f97576f49bbdb918da9bb1afad739003d32b55005a1f756f2b8334b25010e4d19b1042c5c8c2bd3ebfe10d0c67e37350213ef27c729e4ee97b8828aa" },
                { "ta", "5b1de431c5cb42f35c6b8f4149f084dab2d7e3c18997b1557078409551a0ddf79d9482de9f0d74c4bfd69e72b081ba3f60adcc3abcc9cd203e2c3b0132ac9a1b" },
                { "te", "446bcbc9ba996b6eef82c69c8afbf0738b1fc8136440f2a05a76cd0943d453c3d70097ddc098ff27edc0a3b281f22549352a54b7b7030760da004804ae661bb4" },
                { "tg", "e6ef9db94842776f3668927a6e80e8dcee5fd8ea5b52288004ffa4e44cec05785d93b07287ee9aaf2bd882b0d38060423ff7ab977093eeba8a3f3cb7e1ff134c" },
                { "th", "f512b22aa47ee60edbd7931c7ff3411d0210dc6e7fb32c4e4f92b6cad018517f3edcbc289fa7398792594c1ec79dfbddc24cb658995c7df22c08c9438b0e7ff6" },
                { "tl", "aa25bf18acbf637050491513dcc9d260b7d459c5f766ad3043eab29fa471da5a8a9b0938b90f390d18f477c71748c55985aa9ed54743adc81812f205a8be91cb" },
                { "tr", "77188c8fddafcc382642d43c61d3520101d9cc28538df078f1bba9de3ec40d3f101d5f1826db64f8fd920f92eeae7a57492653642eb0193acb186b1012ffdb7c" },
                { "trs", "a13c2393e8c9af822ff609e3d68f3f410c8f411aadd5108a2dbddbcfd397c677c959ff7b5bad6c5b050759c2bf16dad493540f001471c59a35f6f63c7eba35fe" },
                { "uk", "0e75c4ade53548e327990e151a05fa7388c0034e501d48cf5cc9593704a2c2a60e862a26513ca72e788386263eec1a4307656b1f83b38764943379695b60a3fd" },
                { "ur", "cbe007f40b773d2b86c7dd23eb221f74fedc8ef8882990df373a4b177873c60cd9671cd3ff50bd5874bbd230f7575f8d51f89c6ab327cfb3caac9d4790c63281" },
                { "uz", "8d46007ad9f1b0f87b00f1aeff54b97ed2576ffe5b5a2903ab8cc3e4454dc56c4f9bc975fa20a5ad8aebad2aac1e23502c4a1689fb8ae3201fa7041834747b10" },
                { "vi", "1df8b2671e4af080070cabc235eae8bb06a5c0b8a5d1a1838c3773f3d055e57c13c4cd1f28b5603a8914d7ce29d0d369202b0c7bd45c1389a8a284c395484a53" },
                { "xh", "4642ed8118ef52c135ef816eb9c01d486b923d59e2876c611fcb892cc68078bf135666f7c25aa9319cf407b4c68edc35763ca91843e76dc31abe781f692d46c4" },
                { "zh-CN", "dfabd684708687a1d3144cb3ad050941feb8429132e328f37e07cfdca6ec9e4da039b057d57ad3f9de90a193f92019b54eb9d0be7d995d5d6f6c4a3cc6f3225c" },
                { "zh-TW", "a99c4f55b1a710008fae8194876fbd8d36aa4307b328b69268278c39d3248746d3db5168cd407d552d87b3873f72da1ced0727c8b25bfce36d35a4804614a252" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/114.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "80a6e3782f36115dbbdaf76cfc8d4c67f3eb59d522148a6a90860f8058dc02c22a5941fd8beabbfc7cc1d165fbf8473b43bbba04ed4fed49da45c09b89b0eb31" },
                { "af", "4db2c4d517b277f1a79bbcbddc3fe42497702d38c19257c87c8c0bb4f04a0d93dabc51f1fb50816f486cf31ccb8f617114992f8bc1311eafa35659993aa1e55a" },
                { "an", "9c18af703fb50edb2037c650d2885497b48671d273e1801252e72b7b130066ca23df1b9d59420679c182d3c2eeecf40b1237b0159f25cddd32740bb531319098" },
                { "ar", "53e8dcbc9de608acbb818786b19792afb86e0056dc670c5e6e0e33a8427c49ba15fedc5539cda93841beaf2b58442b9e6ec585f6eae72347f5b63654aecbaf64" },
                { "ast", "4be0b4c87255a0f6bda74d8bfaff12b41ce473294d27fc0efd88281c109cf3e38edb0b0a1580be962f45fcff25ed3f6117fec2f581e2730d49bf9b3e0e4820f0" },
                { "az", "d9d4a12437b69b0affca5a64bb905bcf211c6772be610fb6abb9cdb60999da1103e71c4dd6e60abd614598612ff4d82ee8a61605dddb508d011fd5497d43cafb" },
                { "be", "864b79fe1e4f93e1fb15adb944bf1e205934a889f48e39b2921d4b46aea082cf887d9e6a9e219dda6f32093a3c9279d3b52c804fa7112e0f4bc4fd0db1a8ccf8" },
                { "bg", "13e844fd262977bb678a79d99af68787d69d54ca7bbc4224cec76c7c2be83252f2dff8b1d28784256df88ab0dfcc3ec8aa9a34766659cd6ab70a1f2c34706294" },
                { "bn", "1a343d5201c65de62b16d8b6e92dc6a60fd275475a645c6177fd74d4484b5aff2d4b9561c9f57abede52dd11aa4a8e97eb7f953732fa3f6ec8365ea53510d78e" },
                { "br", "56a4682452fd96c344a02ed0663cb7ab0d2602ddb293a36bc9439bdf5f4d3fe525bd8e856a0a26efded7d22e2e135757fc48816eeb6414f4681c713e4b38e8ac" },
                { "bs", "5fa5a33b912ecef57e65f27101cb1aa85512a77d27610fb580e8f3e51a69bb0709864983af62423694664da359aaf4f8ca4056c009c9e02016f8c94ac610c7fa" },
                { "ca", "45f0a5600d106b3601b454a4b551de4c31a1a88f75ea5de8bfefc43a3e573553d5d0d2fcd416958c38c08c8a8179822a6b66446a629013283d38ded0efcf2ad7" },
                { "cak", "a8dbc9e77ac644fce73a8ca56fe9c9914000f10ed7c293967b41afb4d6e3ad3d87f208880c103854db98d8adbcdc8830d73bb39113a6705fb4d75be3afb3ba35" },
                { "cs", "16f1e45f931e89da7b8a54bfc39c907fc0c7b988eff397594c2343c69e56db7f3113935bd258561a136bccde5ef938eac80a6fc52abb2732ab179b859d0f59a2" },
                { "cy", "fcb6499c363249bf196588feac1678b2669e0889fe51660e35d3bd7e4e3f6be50aa823bb611c1fc2785c389a4d0079827f99725dd1cca20c6e427f91489ccae9" },
                { "da", "27c43899c8ae23d4fe6ff118a7e09c263325e6f830a739ab83ebbc46e0679bfb5f8cbb5ba2a4f8149e5dbca556c71ef25b98612aa9fbd54275ffb69fe79682b1" },
                { "de", "fa6e41648b1d321fee01cebbb7f19cd584c0f7ed8b6ebb0018754e853e10991e2ca7745e1d9ece83ffbac187fdac6ff2a38fb4c4a5d6e5d49f6fbb706671509a" },
                { "dsb", "d0c61b395acd296ec6e2c2cefcc5e9d8ee2695c5609f290ee0ff12b05bb0c54aa24f103ad9719b14bfe911a7303c2df725a72253ced0320f67ff5a80bc93dd25" },
                { "el", "da391549ac0b6de1aa192d0b70faf6914761eb50066abbb2a2803bd614cb9ef5f3c45860c679f0507dcc3965308b4f17629a39b17efb0b4959ce7d546f0e754a" },
                { "en-CA", "c9f0b13b6f25882e168d525d33497777bb2f9876bce53001978d22b4808d662da438f0031d2cf868d6e48b1d425a65ca452e82ae50a11c581e2eb407962ec1e2" },
                { "en-GB", "a3d28ba7864efee3c8023834bb2b3c53290ab975f8eaac33aafe210ec70ef22deb9fef34d46d5016adc921fbb288167fceb9b7042cdd52e85d31772736b597af" },
                { "en-US", "12701829542ea4d06338dbfc39148a480a68f21f5a2dc09c35ef3a44b2ec86dca46efa5c52b077b6c89f8fac5f19cafaec2fbc8c627de46880a8e0a8187c5937" },
                { "eo", "5094d982cba8a874137f38e5d6ff9655a5d8766892678079e5566945e1c6773932039bca90b3c2b9988ff03e6bd0963051c6459a90640e99c57435521cb2c2b7" },
                { "es-AR", "1f2516e9c3963fd1f7d038861dd807ced55d530d3a465dab212f99585f26b45d8a93bdd06c60cac0f7b2c4c49c3544fe99ef3aba0cd66fc65095a88ace3d015c" },
                { "es-CL", "946e5df0456d94842466bc4f874d2c17fa1e94819699a53a0aa87f1043f4dc4886184db72b5fb4dc7301d6421320ef39372008420c1030b2254cdd9b4b0e5252" },
                { "es-ES", "b48c79316cac254255f4dee64f27f8b9e4116fe2b2a0747aa08a2abb957b09c459322f633b32e41bafdaa598b139cc1bca8bb263a9520b86da115be86e82133d" },
                { "es-MX", "0791fec53aa724f3417c3e7adcab50cfa5ec27b560811d323e2f9bd1bfd7d9ae1219fd0345cddae86121182a5da63e28b97c1e1664899def4b74a3f27a871d62" },
                { "et", "620cc79e0cc2f63a1faa1bfada47c2c49ea6003f8df3064c6becc2b97270e8b32416ae24c41233d52a80559ccb7978596ef7b81d139e894a2ffd6cef247752f4" },
                { "eu", "5dedc35cd1d7f7d671fd6b90f09006e64471f85624232fb53e45190c3fdad36f3859104ce89306703aaea4288e4962f93a6d31f60158fef936e931c1503db479" },
                { "fa", "cf4bbda0ba1328095d267aafdd1bd8491d8e4825a431716c5f0969d60cbfd3b864c9d8c941c510a21138e29f41f0a528dc3cce53a0be705e1020e3ac2f20c2db" },
                { "ff", "05da6464abf449c6cc2e2e6af0c77d3fad9953ef485d7416bfb8dc7596a78758bb339e549ec968944b1a049c12f4bfb1e54f4cda3cd7dac8e4bb9215fa03bc82" },
                { "fi", "07d1d1e79e3afd918d6f43740ed0b8453c907a5df775e22323aee16f49f9a80c94074862ae81fac954f26434e70743475ab62721d448b6d7a7c4bae0513afa43" },
                { "fr", "6869190f1508cf5af3c1f5f78d47c9b8b9e15f3a7e4fd31558c9e19183987b7e557435b8c2a3b2e6ab946f451b404b8147f5bb5f4f41cbe7ef631ccc20b63f13" },
                { "fur", "c498a8126a2810c5a3b408a05d17d0e4ea48955c12ddb5007b6119d13bcde8abdb7f20ae16da3f4cf4c6f8f318d61b137e73f8e1e802ff1e6e3b67eec209702b" },
                { "fy-NL", "d08fe4c8b12de27d25b806fb5037d52ce185d89ec2976cc2510f33ecbb85bbfbebb02182fdf0a1530ff5abee50d6aaec101ab212033580ec3b016889ac48310c" },
                { "ga-IE", "32cf4fc53e4c595c9c1a806f86c2cf11671f5603addbbccff4759b23bdd7d9f9a29d4580969a51e75b947d7bc125beb82b445a2dcc0061d3d39a331a7d622413" },
                { "gd", "bec801058d83c09ffc80989eb0420cb2612dc5c0ee8f384ff1d6b2e7058a4b95c2e969c9687bd7662aafa448f0dda3b8c6f977fc0f2ddab49770296640dd23f1" },
                { "gl", "8bef4f6881beaf7d39fbf63652f2f863afcd271ce52eb55cf5f0a85d624df3dc0d37ea4c6230080e8c202e73e8f23d1d9ea53cebc87342cbbc65ff8632d5ba1f" },
                { "gn", "6f24a0d26c78455460a0f6d9a380fe724073109063a222c26f624436db9c08abd10693d14cd471d75a90e76e59d5f79a087c9c225a3d3e0cd389b01b136d1abf" },
                { "gu-IN", "c46660352af22fd3ace668e33ff3f38e44e6e44bc3c4c3b54cb43ab73d0358e4f8a4d911acc841540f194dea7ec83223606c076fa659569b18aeb432a84ff1f0" },
                { "he", "d36d471a9add2f32a6ddd94103384dcdc98772b043bee2fdb6a6a6fd69af39f83a9d17350c3f28abf029c876db52290f4723f231b98a7e853b3f897d03803563" },
                { "hi-IN", "32fdb0bad9d624c1053b242efbeaea626a49e6c80112986b321767ef46cab5785fbef0f4921c6f8db31675fff2ec114b1e223dae313985b40158c24aacdd1cd7" },
                { "hr", "64355af2921f0e0ef837434610116badafa5b27a6791359d94adc813c9fc4d52bc822be5ef127f312792d6baba15491516228159fd407d3bfa254bf1a7fe45dc" },
                { "hsb", "f060b4719440a930b505378e6eb20091704b80b2ca8b36a93b8482dd98558937b8c3355b0618ab7da6b24ac094732f12124ce8e39ddfc1c7bb78aee6dccb7052" },
                { "hu", "ed2641d8f873b26ba97514808e96499d7ed30e3a595b5cf444ab45d4fc7bdbcbec4b8adef79d0f18101b8c8ac5bf1752860762a0ef53cb15d617529c38c35984" },
                { "hy-AM", "f31777fe6c243d33ea7e9b77390c05de99996b3dd3a30bd06b47fe65db2a0c416fd66dc7ed0b4bb99fcbec0d5550726d2fb3423be9d44f99172c3deccc3c3d68" },
                { "ia", "622d1143c73ea733562d0b95a499221bfeb91b6792c0392872616c1375b946de2c07b6f6533fbba39f5a2ed5cfce71bdfcb816d39529d25c5b9f7897e5dc4771" },
                { "id", "d33cb7cfdf37d9c0ab21aa6a8ffc518b555f6184cc5a25af63669a0140327fed50cc47485c05833873da9ff7a9d4add03ecd87c924caff5dd596298134a92223" },
                { "is", "fe7ce46ac45dd1ce8d551575e8f1c326258ff49e60bfe6ecf078692478643895d2e73d52706ce2bcd11a20a646e0f00957bd1333404491a9a2a37abe627521e6" },
                { "it", "08321621f9993dd0eaba2d1014eb6694f718111846ae15232deb05d78e4ba2760d2190b58ed0a7de1cb1dd674d4957b5926cfa7550e23a25a22e634ab4c6f5b7" },
                { "ja", "f3cd19b90e130c5d7280e679663484a6d1c3c31a2d436a836b910a91611cae5af972796d34850b50e8ad8465d6ff92e8cc2ae3c69208732ea52a6b38376448b4" },
                { "ka", "16fa8e3722f6c970278a7e2a073a95644699d081998b1832513fc88679c93f5b1da5e090115c470cdff8bc28dd9ce8275a6a56f3dcc03d093889b79036693112" },
                { "kab", "b50de389af37791d327af5b6c6101ce94034aac426464c113616d018a19d0078e7b1572ef4129d5576303a98728bd2ce19793b0b9e24faf46ffe8155e2d3f316" },
                { "kk", "3ef7e46a84a6fbba78fe590ccde476701d8ae1dabcb0d0853e43f45dcf79e09cf6dc964065885627aeb403fda64a3788e2397354c9eeaa9089a744a1de856475" },
                { "km", "2c72dcd3a125713a839292d8e996bc3da82e69fb2a29eae6935229dae9a6e98dd7df977fbc6e32ddde36184329ea2c3ba064c6012ed7b5f97a4bfc34b14b8e67" },
                { "kn", "9daa3a69e9de80d410001f439420928c11ed7df6d13df9fcf7947489e2e2cd1b576985304d4d2d82362480b011afac802553e7986e8fba8151673e01672253f6" },
                { "ko", "dfc6d9635cb1e49a5919346c8ccc7d4b26df2721153ab775b5e3cbf15d5f9c785d751f42d6e30846f76defde77b4d7988c9c94c76a5a10c08cce9c6c9e043da3" },
                { "lij", "5177e542e6b0c1c3e549f534066e07a12067b3c9ec36a7b5f5b7f27392483597d56f43824b0737f31f2d8ccb056c1ace259f894f0430debac18a7a7d7e240a9c" },
                { "lt", "f896171621f7127240139fbd23fc85406c1a3e21795a591a1753099029ffca370d6b206afe815ec6578e5012a83bab7a6d9f6e32d38b3ba554efce730c5ba6dc" },
                { "lv", "08d93c4314e34a5774e28f4e8313e05f2739efcd20428308282ff8ca199007f25a293b43206226e260cad66ff89e947042ca864240652ba51df2cb6103c8c79a" },
                { "mk", "cbfd7a3b8b4a1636388f74796ba12e1113f88c4c35d726f6db557e5c0d4048e2385283fc16afc7812ce1f70d4a297c08cb89d757a4bdb531ae41b74a9e84aa4c" },
                { "mr", "01aefce9887e839ce9a6a34e4cec90505a149aa13fe1486a50afe2835c22811b21eaa04762cc4b0bde4d955826cf85a100dd0fdd2219c68e5c2cb51cbdda5394" },
                { "ms", "d9fbf95293f82a2f3b0713940d35ff652174a4cd9bd3fb1cb452e5bdcf7fd2c5fb6c438dcbea15876c1a2fee5fea149bf70af2cd9002839813885c4d74af0a3a" },
                { "my", "d861d65a8f6cbc6ac9258b45d1bb2a292f369cfe58fd05100ca4906c21aae9b4d0fa539ba366086de11da692f928403138c8a6aa6c7596ba01b52f29c2d35a88" },
                { "nb-NO", "061081e02c3c03844d8a8bee035503a2d430d58e4ec2afc5d336482602c512ae8d31fd76f740a8ad42516ee39417612550311a2609d3c9ed00ce060c2f5cf6cd" },
                { "ne-NP", "777e8c486d40df53058bccf0c299ba3c7128a476729f459cb46138c22d8ff3183f10673029fcb7049f9665e519a7c0bd0ec9d8ee06b42916a78ab4e629911da0" },
                { "nl", "645dd8c561251d97e5d9f8295e01a90e3a6c30eaf9ca7bad677d7110a10c8316bde113009776e6a64a359db4c0c381ccbc6232fa68d2a96a18b186f9c8aab989" },
                { "nn-NO", "5e4ff9be0cb5494eb24dc3a65633a989b1007e4ff2b40583c4bd65129c99a8fa23531cf86116c72b23e0d27928fa13d617a94770602be82ac861036794457661" },
                { "oc", "82027a5032d45551e2d7ecfdb2efffafba75cddc9f60d355b5eccd877076d6643893242383d8cf970a985bb0450f75c5b4cba53a18b0185fc87846f910483443" },
                { "pa-IN", "a1ea60bdf7700aa6a572d751bc56e3f7340e8408c31c4be952c57b88f80998563280308e2277805c4e2a551e420bbc023489709e8052b9609210061b31c4bfe4" },
                { "pl", "e944f4f89a9ee296b3908545779294891830ed24e44783b66099d4e44d4325d2806a4e5039dc645d84aa60c9d134baed3c591d1864adc0aaecc8665507b2fefe" },
                { "pt-BR", "b388647544434f506878195a09cd7844e3d0e6d40242dcc10ed89439268907183b98659ca56c83566772bff6196aa765d47e0df75801b36d04b359c17797785f" },
                { "pt-PT", "5369de59a382ad0c2d2b011c979346caacea5f75e920a694b6310112061dd2dbe0d070da80e6d8623a25ec728c4f1e0c5470693f6d5e2b0a71dcb62ead3e696a" },
                { "rm", "14d19895a1e45261aba256a2fa6f77903f0a47e5cbe94e9d66d625d18d8d7b3d7e00107b6305098b96c4303371ed16974e6bf2c0c055fcfe643bd2591331fd2d" },
                { "ro", "91985baaf650881d46c570057d7ed0d8be0603675aca834c5bc71f8141ec8a8660e6f895eea3ffbc41217ee3123ee54631c8e988508eb1951b349fdc2bee620a" },
                { "ru", "4efff752a40fd5f9b081e3056f82635cdce7238ca56d9dc47953cab11177881c06c4d496cbe1454a7433e78ac580e54ebd551d9b5ba8f05aa89e2e1f14f35b21" },
                { "sc", "01258d4bb6dcec2ef55cc279a83ae4b830b2a08e7e1336763571226f41881eda4bd6b87b29a18f6506cee0a6b6aaa0990871237cbbcbc3acb46d52c664039729" },
                { "sco", "88d6c4d8477f2e142297a1f3cc64a7a67ffa40512af19df6710eb8cbb6b32a6b3223c71769b1473cc751a381b8ad815e91bf9d68e2a9c03c711bc75c3c576da4" },
                { "si", "2fb2fff5ca62693c8b367637f2099515711eab51da1975a3e4c68651115c3662fb01a1a315153ce082239c1d4d199af6fe585e82b0fdbabadb9ccb6f1228a260" },
                { "sk", "eb24883d1624a727048a40c0cd7fe80886e684d50e5d10c1c66d597d08e4ceb6d73fe12c544c50a2e7a15cf38bb956b24bfe795522c391cee00765131b1b2b37" },
                { "sl", "f51c00b7c9b0f58c89b48abb26b990dd3621d7b9561e2c327f683cabc8dd67b4ba0ab6e51421dca89c72595b82902c91538f4ac4063b09105811c2331a9c82ec" },
                { "son", "91df9ee5ac92886d4055ec3dfbb17da98b57da490af1669d3ae7d0a3ee23aec3fce7f319514c434b51698ced624ebd560888d9f7ca50e749e2530f621f4ce2a9" },
                { "sq", "2c339905959eab1b71644943d01d54e5fc1c454c05a8c21eb4ffca517effdc1363fbeb47f03392c91159123e48d5f314bde0bc7b250dc219f617546940db7318" },
                { "sr", "81ad971a8c810ccf9e195c3c1e389aed8bc24b2a35e46c5a25668a93b4256c57366bb035b721393bb390d7cb2cb9389e6382690bcba65ff8b776550f0fd1dace" },
                { "sv-SE", "61ec9d400c4a0d36b9fd5d32959fd5992dda40a3ea8ffeee731d1400053e06f1f51087543155d38ae16f3f478f520f08396a62594c3132f2f589502c8fd7ca88" },
                { "szl", "f5860de640f2d304a190adb9c60ba12e1aa770eee13c947e1c2390d92c64eeab381872a0e58052d6da70d6b3046bc6613b7b2450dd52b1f6fc81aae919c2714a" },
                { "ta", "e9fb65fe3119bb62cb07748e0bb5c4dcd66288a96ff25236a3b921448038489ef2252cafe3967bac69a0afebb9fb53f7ed18a69ad9ba5e84b8f92ee428034028" },
                { "te", "1c387f9a7345d9725e4a249e3fccdd3c15e21e8cc7528b4fbf9dc03a209f9fcf0f8301ba5447bb28285009d363a8b3f98d8b6a47908b9db90b2f84fc81b02d0e" },
                { "tg", "dc0d465a0615cdc041618db5e59fc3569b1a64a9478ade9865d8cd5caae32c93391a562c1652102e1534487236f64902e27e048a6cc683c7841a2e6ef49edb96" },
                { "th", "2fbf7910556e6718264fdfa9fa4a52f6ad2f4f8a96821235cc2879e340d14d87fa4d0162abe155404b62c694b6026c176f70615c9333ba5e4a7fbd476dcfd34c" },
                { "tl", "394297aa712454ce2cbf1ae8c6b0283878acdd0912ba91975ca8c87570f12fe7ba20f7b21c468c28713c190373b7516d79c20dbf3a002fbf22e164a6ddadac71" },
                { "tr", "983686214de0df3579319be9fea3ebc9716f16d83619287680534d36d786e8f8632aac1824e76e49196980b85285fd93ce81215e89d12698d0cd06d72b011955" },
                { "trs", "228041ac997678f9425a7953ebd05a5836c90a98947752837fee8603ef38602cd32a7b4f6ee5b03210a583a8abd2c512c71920414655b2f833935ab29cdc2eea" },
                { "uk", "61cdfa6a95aa37e0e239992172bc5f55f4b50559b874a5ba152db359797fdab750c01afa78942414bdf2391665da2296aa44a252c1d7cd18ebd333e7e1383bb7" },
                { "ur", "63955dd8581efb3cd57fbcba83895df85621a425ceef5b8048cd79b106d7c6bdbd1b6ce810fd14b03fef86b36e62979b06a8998d790175479e7aa8e0f6d4f37f" },
                { "uz", "202664088997b51e7c5f6fa8028dc8087f51327056ca6fcb7960f52c282285c72ab5344b732ab0a003e09fa9f1c55412e264eda84f10b795fb3447d8b54cd264" },
                { "vi", "ad62b27125e11d527c5ff59994829825b50ff37c290b3b999cd430b82e536d3e53dc5bc088f8576bbb559d927b73d70956bc277b8c96a3d54124abe4884d238b" },
                { "xh", "88094f9efeca236448fef66edc9a9cd70986e3533ea52a51024ed3158172d770a7d666671544e580195bac298da42ae3ab4e6e2746b25bfe53eb0c2d1019dda6" },
                { "zh-CN", "1b5e9874395725856e578842aa9627c32e91cd0ad50ad78cb11532305f62d0acdc32870cb23dcdd2b990ca1c2100acc193892f2701ba4901e2b4fede4d52b6bc" },
                { "zh-TW", "c1f27e722fcc9f20ce2d81a8aca37b587929226a3ff9f7f917c388cc989ccd60fc01a036a49d851e9cd9c591943250222fe4f696f3ff2783e1304163964113f0" }
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
            const string knownVersion = "114.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
